﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Automation;

namespace PurpleLib
{
    public class PurplePath
    {
        //This class is used to build and interpret purplepaths to find AutomationElements
        //These constants should be stored in the app.config
        private const String DELIMITER = "//";
        private const String BLANK = "!BLANK!";
        private const String DEFAULTWINDOWNAME = "EMPTY";
        private const char ORDERSTART = '[';
        private const char ORDEREND = ']';

        public String DefaultWindowName {set { _topLevelWindowName = value; }}
        public char ValueDelimiterStart {set { _orderStart = value; }}
        public char ValueDelimiterEnd {set { _orderEnd = value; }}
        public String Delimiter {set { _delimiter = value; }}
        public String BlankValue {set { _blankValue = value; }}

        private String _blankValue = BLANK;
        private String _delimiter = DELIMITER;
        private string _topLevelWindowName = DEFAULTWINDOWNAME;
        private char _orderStart = ORDERSTART;
        private char _orderEnd = ORDEREND;

        public PurplePath(char orderstart = ORDERSTART, char orderend = ORDEREND, string wName = DEFAULTWINDOWNAME, String delimiter = DELIMITER, String blank = BLANK)
        {
            _orderStart = orderstart;
            _orderEnd = orderend;
            _topLevelWindowName = wName;
            _delimiter = delimiter;
            _blankValue = blank;
        }


        /// <summary>
        /// Finds the PurplePath based on the element found.  This will return the entire purple path for the window the element was found in.  If 
        /// Default window name is turned on in the UI that will become the top level name under the desktop level and ignore any other text in the window
        /// title bar
        /// </summary>
        /// <param name="element"></param>
        public String getPurplePath(AutomationElement element)
        {
            //I was curious how UI automation would handle having more than one panel with a blank name from Inspect.exe when building this path
            //It was surpriseing to know that the TreeWalker handles that for us when we walk up the tree, and conversly down the tree.
            
            TreeWalker walker = TreeWalker.RawViewWalker;
            TreeWalker walkDown = TreeWalker.RawViewWalker;
            bool parentExists = true; //need to assume that there's a parent
            String path = element.Current.Name;
            AutomationElement parent;
            AutomationElement node = element;
            String purplePath = "";
            string parentName = node.Current.Name;

            //HOLY HELL - FINALLY got this working i think
            while (parentExists)
            {
                int childnum = 0;
                int matches = 0;
                
                //get the parent item of the found item
                parent = walker.GetParent(node);
                if (parent != null)
                {
                    //list to store children of parent of item
                    List<AutomationElement> ChildrenFromParent = new List<AutomationElement>();
                    bool childexists = true;
                    while (childexists)
                    {
                        //get the first child of the parent of the item
                        AutomationElement sibling = walkDown.GetFirstChild(parent);
                        if (sibling != null)
                        {
                            //add the first child to the list
                            ChildrenFromParent.Add(sibling);
                            //get the next sibling
                            AutomationElement nextSibling = walkDown.GetNextSibling(sibling);
                            while (nextSibling != null)
                            {
                                //get all the children of the item
                               ChildrenFromParent.Add(nextSibling);
                               nextSibling = walkDown.GetNextSibling(nextSibling);
                            }
                        }
                        //stop the 3rd loop when all the siblings of the item are found
                        childexists = false;
                    }
                    //check the names of each of the siblings
                    for (int x = 0; x < ChildrenFromParent.Count; x++)
                    {
                        //Check if the name matches
                        if (node.Current.Name == ChildrenFromParent[x].Current.Name)
                        {
                            //increment the number of matches
                            matches++;
                            if (node.Current.Equals(ChildrenFromParent[x].Current))
                            {
                                //the matches will correspond with the number of matches -1 to give us a path were we dont' have to do any evaluations if there are no names exactly the same
                                childnum = matches - 1;
                            }
                        }
                    }
                    //check to see if we need to add on a value for number of items with the same damn name
                    if (childnum > 0)
                    {
                        path += _orderStart + childnum + _orderEnd;
                    }
                    //add on the delimiter
                    path += _delimiter;
                    parentName = parent.Current.Name;
                    
                    //now we have to find out if the parent name contains a blank --another great 'feature' of windows UI automation
                    //if the first char in the parentName at this point is [ we know it's one of at least 2 blanks
                    if (parentName == "")
                    {
                        //We need to put something for blanks
                        parentName = _blankValue;
                    }
                    
                    //now add it to the path
                    path += parentName; //Dont put the delimiter here
                    node = parent;
                }
                else
                {
                    parentExists = false;
                }
            }
            //now try to build the path in the proper order
            string[] pathStrings = path.Split(_delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Array.Reverse(pathStrings);
                //Reverse the order of the strings in the array so elements appear in top to bottom
            //this trims off the first level, since that's the root element or Desktop
            for (int x = 1; x < pathStrings.Count(); x++)
            {
                //This if will replace the first value with the default window name if provided by the UI
                if (x == 1 && _topLevelWindowName != DEFAULTWINDOWNAME)
                {
                    pathStrings[x] = _topLevelWindowName;
                }
                purplePath += _delimiter + pathStrings[x];
            }
            return purplePath;
        }

        /// <summary>
        /// Finds the AutomationElement based on the PurplePath provided.  This will return the element the first element that matches the last level of the path locator provided.
        /// If DefaultWindowName is set, the top level window name will be matched only partially.  This accounts for window titles dynamic titles based on file name or user.
        /// </summary>
        /// <param name="purplePath"></param>
        public AutomationElement FindElement(String purplePath)
        {
            //This function will return a AutomationElement based on purple path provided
            var pathStrings = new List<string>(purplePath.Split(_delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            //RootElement starts on the desktop
            AutomationElement element = AutomationElement.RootElement;
            TreeWalker walker = TreeWalker.RawViewWalker;
            AutomationElement node = element;
            List<AutomationElement> childElements;
            
            //We only want to browse down as far as we need to
            for (int i = 0; i < pathStrings.Count(); i++)
            {
                //The first path name will always be the app main window - we only want to match on the first part of the name since some apps will put the file path in the window name
                
                //If the path string has more than one child with the same name
                if (pathStrings[i].EndsWith(_orderEnd.ToString()))
                {
                    int orderstart = pathStrings[i].IndexOf(_orderStart);
                    orderstart += 1; //the length of order start
                    int orderend = pathStrings[i].IndexOf(_orderEnd);
                    int length = orderend - orderstart;
                    int value = int.Parse(pathStrings[i].Substring(orderstart, length));
                    //find the start of the ordernumber
                    string name = pathStrings[i].Substring(0, orderstart - 1);
                    int match = -1;

                    childElements = new List<AutomationElement>();
                    node = walker.GetFirstChild(element);
                    while (node != null)
                    {
                        childElements.Add(node);
                        node = walker.GetNextSibling(node);
                    }
                    for (int x = 0; x < childElements.Count; x++)
                    {
                        //We only want to do this fancy crap with the top level name since that should be the window name under the desktop
                        if (i == 0)
                        {
                            //The first path name will always be the app main window - we only want to match on the first part of the name since some apps will put the file path in the window name
                            //There should really never be two windows with the exact same name but i'm sure this will come up at some point
                            if (childElements[x].Current.Name.Contains(_topLevelWindowName) && _topLevelWindowName != DEFAULTWINDOWNAME)
                            {
                                match++;
                                if (match == value)
                                {
                                    //we found the right one
                                    node = childElements[x];
                                }
                            }
                        }
                        else
                        {
                            //handle the blank names
                            if (name == _blankValue)
                            {
                                name = "";
                            }
                           if (childElements[x].Current.Name == name)
                            {
                                match++;
                                if (match == value)
                                {
                                    //we found the right one
                                    node = childElements[x];
                                }
                            } 
                        }
                    }

                }//endif More than one child with the same name
                else
                {
                    string name = pathStrings[i];
                    childElements = new List<AutomationElement>();
                    node = walker.GetFirstChild(element);
                    while (node != null)
                    {
                        childElements.Add(node);
                        node = walker.GetNextSibling(node);
                    }
                    for (int x = 0; x < childElements.Count; x++)
                    {
                        if (i == 0)
                        {
                            if (childElements[x].Current.Name.Contains(_topLevelWindowName) && _topLevelWindowName != DEFAULTWINDOWNAME)
                            {
                                node = childElements[x];
                                //we just want the first match here, since there are sometimes hidden elements under the current element
                                x = childElements.Count;
                            }
                        }
                        else
                        {
                            if (name == _blankValue)
                            {
                                name = "";
                            }
                            if (childElements[x].Current.Name == name)
                            {
                                node = childElements[x];
                                //we just want the first match here, since there are sometimes hidden elements under the current element
                                x = childElements.Count;
                            }
                        }
                        
                    }

                }
                var nodename = node.Current.Name;
                if (node != null)
                {
                    element = node;
                }
            }
            return node;
        }

        
    }
}
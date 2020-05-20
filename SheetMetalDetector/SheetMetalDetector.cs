using NXOpen;
using NXOpen.BlockStyler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetMetalDetector
{
    public class SheetMetalDetector
    {
        //class members
        private static Session theSession = null;
        private static UI theUI = null;
        private string theDlxFileName;
        private NXOpen.BlockStyler.BlockDialog theDialog;
        private NXOpen.BlockStyler.Group group0;// Block type: Group
        private NXOpen.BlockStyler.ListBox LB_SelObjs;// Block type: List Box
        private NXOpen.BlockStyler.Button button_Verify;// Block type: Button
        private static ListingWindow lw = null;

        // Custom variables
        private List<string> allPartNames = new List<string>();

        //------------------------------------------------------------------------------
        //Constructor for NX Styler class
        //------------------------------------------------------------------------------
        public SheetMetalDetector()
        {
            try
            {
                theSession = Session.GetSession();
                theUI = UI.GetUI();
                lw = theSession.ListingWindow;
                theDlxFileName = "SheetMetalDetector.dlx";
                theDialog = theUI.CreateDialog(theDlxFileName);
                theDialog.AddUpdateHandler(new NXOpen.BlockStyler.BlockDialog.Update(update_cb));
                theDialog.AddInitializeHandler(new NXOpen.BlockStyler.BlockDialog.Initialize(initialize_cb));
                theDialog.AddDialogShownHandler(new NXOpen.BlockStyler.BlockDialog.DialogShown(dialogShown_cb));
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                throw ex;
            }
        }
        //------------------------------- DIALOG LAUNCHING ---------------------------------
        //
        //    Before invoking this application one needs to open any part/empty part in NX
        //    because of the behavior of the blocks.
        //
        //    Make sure the dlx file is in one of the following locations:
        //        1.) From where NX session is launched
        //        2.) $UGII_USER_DIR/application
        //        3.) For released applications, using UGII_CUSTOM_DIRECTORY_FILE is highly
        //            recommended. This variable is set to a full directory path to a file 
        //            containing a list of root directories for all custom applications.
        //            e.g., UGII_CUSTOM_DIRECTORY_FILE=$UGII_BASE_DIR\ugii\menus\custom_dirs.dat
        //
        //    You can create the dialog using one of the following way:
        //
        //    1. Journal Replay
        //
        //        1) Replay this file through Tool->Journal->Play Menu.
        //
        //    2. USER EXIT
        //
        //        1) Create the Shared Library -- Refer "Block UI Styler programmer's guide"
        //        2) Invoke the Shared Library through File->Execute->NX Open menu.
        //
        //------------------------------------------------------------------------------
        public static void Main()
        {
            SheetMetalDetector theSheetMetalDetector = null;
            try
            {
                theSheetMetalDetector = new SheetMetalDetector();
                // The following method shows the dialog immediately
                theSheetMetalDetector.Show();
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            finally
            {
                if (theSheetMetalDetector != null)
                    theSheetMetalDetector.Dispose();
                theSheetMetalDetector = null;
            }
        }
        //------------------------------------------------------------------------------
        // This method specifies how a shared image is unloaded from memory
        // within NX. This method gives you the capability to unload an
        // internal NX Open application or user  exit from NX. Specify any
        // one of the three constants as a return value to determine the type
        // of unload to perform:
        //
        //
        //    Immediately : unload the library as soon as the automation program has completed
        //    Explicitly  : unload the library from the "Unload Shared Image" dialog
        //    AtTermination : unload the library when the NX session terminates
        //
        //
        // NOTE:  A program which associates NX Open applications with the menubar
        // MUST NOT use this option since it will UNLOAD your NX Open application image
        // from the menubar.
        //------------------------------------------------------------------------------
        public static int GetUnloadOption(string arg)
        {
            //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
            return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }

        //------------------------------------------------------------------------------
        // Following method cleanup any housekeeping chores that may be needed.
        // This method is automatically called by NX.
        //------------------------------------------------------------------------------
        public static void UnloadLibrary(string arg)
        {
            try
            {
                //---- Enter your code here -----
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //This method shows the dialog on the screen
        //------------------------------------------------------------------------------
        public NXOpen.UIStyler.DialogResponse Show()
        {
            try
            {
                theDialog.Show();
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }

        //------------------------------------------------------------------------------
        //Method Name: Dispose
        //------------------------------------------------------------------------------
        public void Dispose()
        {
            if (theDialog != null)
            {
                theDialog.Dispose();
                theDialog = null;
            }
        }

        //------------------------------------------------------------------------------
        //---------------------Block UI Styler Callback Functions--------------------------
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //Callback Name: initialize_cb
        //------------------------------------------------------------------------------
        public void initialize_cb()
        {
            try
            {
                group0 = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group0");
                LB_SelObjs = (NXOpen.BlockStyler.ListBox)theDialog.TopBlock.FindBlock("LB_SelObjs");
                button_Verify = (NXOpen.BlockStyler.Button)theDialog.TopBlock.FindBlock("button_Verify");
                //------------------------------------------------------------------------------
                //Registration of ListBox specific callbacks
                //------------------------------------------------------------------------------
                //LB_SelObjs.SetAddHandler(new NXOpen.BlockStyler.ListBox.AddCallback(AddCallback));

                //LB_SelObjs.SetDeleteHandler(new NXOpen.BlockStyler.ListBox.DeleteCallback(DeleteCallback));

                //------------------------------------------------------------------------------
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //Callback Name: dialogShown_cb
        //This callback is executed just before the dialog launch. Thus any value set 
        //here will take precedence and dialog will be launched showing that value. 
        //------------------------------------------------------------------------------
        public void dialogShown_cb()
        {
            try
            {
                //---- Enter your callback code here -----

                // Initializations
                lw.Open();
                lw.WriteFullline(
                    " ---------------------- " + Environment.NewLine + 
                    "| SHEET METAL DETECTOR |" + Environment.NewLine + 
                    " ---------------------- ");
                button_Verify.Enable = false;
                button_Verify.Tooltip = "Select at least one object to verify";

                // Populate ListBox with current opened objects
                foreach (NXOpen.NXObject part in theSession.Parts)
                {
                    string prefix = "";
                    switch (part.GetType().ToString())
                    {
                        case "NXOpen.Part":
                            prefix = "CAD   |  ";
                            break;

                        case "NXOpen.CAE.FemPart":
                            prefix = "FEM   |  ";
                            break;

                        case "NXOpen.CAE.AssyFemPart":
                            prefix = "AFEM |  ";
                            break;

                        case "NXOpen.CAE.SimPart":
                            prefix = "SIM   |  ";
                            break;

                        default:
                            break;
                    }

                    allPartNames.Add(prefix + part.Name);
                }

                LB_SelObjs.SetListItems(allPartNames.ToArray());
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //Callback Name: update_cb
        //------------------------------------------------------------------------------
        public int update_cb(NXOpen.BlockStyler.UIBlock block)
        {
            try
            {
                if (block == LB_SelObjs)
                {
                    //---------Enter your code here-----------
                    button_Verify.Enable = LB_SelObjs.GetSelectedItems().Length > 0 ? true : false;
                    button_Verify.Tooltip = button_Verify.Enable ? "" : "Select at least one object to verify";
                }
                else if (block == button_Verify)
                {
                    //---------Enter your code here-----------
                    foreach (int objIndex in LB_SelObjs.GetSelectedItems())
                    {
                        // Get selected object
                        NXOpen.NXObject targObj = theSession.Parts.ToArray()[objIndex];
                        lw.WriteFullline(Environment.NewLine +
                            targObj.Name.ToUpper());

                        // Check if CAD object
                        if (targObj.GetType().ToString() != "NXOpen.Part")
                        {
                            lw.WriteFullline("   -> Skipped:  not a CAD object, but of type = " + targObj.GetType().ToString());
                            continue;
                        }

                        NXOpen.Part targCAD = (NXOpen.Part)targObj;

                        // Check if target CAD object is a Sheet Metal object
                        if (DetectSheetMetal(targCAD))
                        {
                            lw.WriteFullline("   -> SHEETMETAL");
                        }
                        else
                        {
                            lw.WriteFullline("   -> normal CAD");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }
        //------------------------------------------------------------------------------
        //ListBox specific callbacks
        //------------------------------------------------------------------------------
        //public int  AddCallback (NXOpen.BlockStyler.ListBox list_box)
        //{
        //}

        //public int  DeleteCallback(NXOpen.BlockStyler.ListBox list_box)
        //{
        //}

        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //Function Name: GetBlockProperties
        //Returns the propertylist of the specified BlockID
        //------------------------------------------------------------------------------
        public PropertyList GetBlockProperties(string blockID)
        {
            PropertyList plist = null;
            try
            {
                plist = theDialog.GetBlockProperties(blockID);
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return plist;
        }



        // CUSTOM METHODS & LOGIC
        /// <summary>
        /// Checks whether the input CAD part has any Sheet Metal bodies in it. If yes, then this CAD part is considered as a Sheet Metal object.
        /// </summary>
        /// <param name="targCAD">Input CAD part</param>
        /// <returns></returns>
        public bool DetectSheetMetal(NXOpen.Part targCAD)
        {
            // Get Features
            NXOpen.Features.FeatureCollection myFeatures = targCAD.Features;

            // Get Sheet Metal Manager
            NXOpen.Features.SheetMetal.SheetmetalManager mySheetMetalManager = myFeatures.SheetmetalManager;

            // Check if one of the bodies in CAD part is a Sheet Metal Body
            foreach (NXOpen.Body body in targCAD.Bodies)
            {
                if (mySheetMetalManager.IsSheetmetalBody(body))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

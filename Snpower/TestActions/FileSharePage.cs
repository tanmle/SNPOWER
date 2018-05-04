using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snpower.TestActions
{
    public class FileInfo
    {
        public string FileName { get; set; }
        public string FileTitle { get; set; }
        public string FileDescription { get; set; }
        public string Owner { get; set; }
        public string OwningEntity { get; set; }
        public string Author { get; set; }
        public string EffectiveDate { get; set; }
        public string Person { get; set; }
        public string Permission { get; set; }
        public string PublishedDate { get; set; }
        public string Publisher { get; set; }
        public string MediaPublishedDate { get; set; }
        public string Media { get; set; }
        public string MeetingDateAndTime { get; set; }
    }

    public class ImportFileInfo
    {
        public string Type { get; set; }
        public string Source { get; set; }
        public string State { get; set; }
    }

    public class FileSharePage : GeneralPage
    {
        private static BrowserMonkey browserMonkey = BrowserMonkey.Instance;

        private string dynamicCountry = "//div[@class='item-selector dots']//span[text()='{0}']";
        private string selectedTreeItem = ".ui-tree li.tree-item span";
        private string selectedActivityLabel = "a.ms-ContextualMenu-link.is-selected span.ng-binding";
        private string newButton = "//div[contains(@class, 'ms-Grid-col')]//button/span/span[text()='New']";
        private string importButton = "//div[contains(@class, 'ms-Grid-col')]//button/span/span[text()='Import']";
        private string deleteButton = "//div[contains(@class, 'ms-Grid-col')]//button/span/span[text()='Delete']";
        private string newDocumentButton = ".ms-Icon--PageAdd";
        private string newActivityButton = ".ms-Icon.ms-Icon--AutoFillTemplate.action-doc__icon.ng-scope";
        private string dialogTitleLabel = ".ms-Dialog-title";
        private string provideDetailButton = "button.red-button[ng-click*='provideDetails']";
        private string selectedStepLabel = "li.step-list__item-active";
        private string selectAllFiles = "//th[@ng-click='$ctrl.selectAllDocuments()']";
        private string createCopyButton = "button[ng-click*='ctrl.copy']";
        private string shareButton = "button[ng-click*='ctrl.toggelMenuForSareButton']";
        private string compareButton = "button[ng-click*='$ctrl.compare($ctrl.selectedDocuments)']";
        private string viewButton = "button[ng-click*='$ctrl.view($ctrl.selectedDocument)']";
        private string dynamicShareItems = "li[ng-click*='{0}']";
        private string menuButton = "//button[@ng-click='$ctrl.toggleContextMenu($event)']";
        private string dynamicMenuItem = "//li[contains(@ng-click,'{0}')]";
        private string renameTextbox = "//div[@ng-if='document.showRename']/input";
        private string saveRenameButton = "//div[@ng-if='document.showRename']/button";
        private string dynamicActivity = "//sims-tree-item[@is-activity-page='$ctrl.isActivityPage']//span[@class='ng-binding' and text()='{0}']";
        private string dialogCloseButton = "//button[@ng-click='$ctrl._closeDialog()' and @class='ms-Dialog-button ms-Dialog-button--close']";

        //About File
        private string aboutFileTitleTextbox = "//input[@placeholder='Enter a unique title for the document.']";
        private string aboutFileDescriptionTextarea = "//div[.='Description']/following-sibling::div//textarea";
        private string aboutFileSaveButton = "//button[@ng-click='$ctrl.updateDocument()']";
        private string aboutFileCancelButton = "//button[@ng-click='$ctrl._closeDialog()']";

        //File dialog
        private string fileTitleTextbox =
            "//span[@ng-bind='field.Label'][text()='Title']/../following-sibling::div/input";
        private string fileDescriptionAreabox =
            "//span[@ng-bind='field.Label'][text()='Description']/../following-sibling::div//textarea";

        private string fileOwnerTextbox = "//span[@ng-bind='field.Label'][text()='Owner']/../following-sibling::div//input";
        private string fileAuthorTextbox = "//span[@ng-bind='field.Label'][text()='Author']/../following-sibling::div//input";
        private string filePersonTextbox = "//span[@ng-bind='field.Label'][text()='Person']/../following-sibling::div//input";
        private string fileEffectiveDateTextbox = "//span[@ng-bind='field.Label'][text()='Effective Date']/../following-sibling::div//input";
        private string fileMeetingDateAndTimeTextbox = "//span[@ng-bind='field.Label'][text()='Meeting Date And Time']/../following-sibling::div//input";

        private string fileOwningEntityDropdown =
            "//span[@ng-bind='field.Label'][text()='Owning entity']/../following-sibling::div//span[contains(@class, 'ms-Dropdown-title')]";

        private string filePermissionDropdown =
            "//span[text()='Select permission type']/../following-sibling::div//span[contains(@class, 'ms-Dropdown-title')]";

        private string createDocumentButton = "button[ng-click*='createDocument']";
        private string deleteReasonTextbox = "#reason";
        private string deleteDocumentButton = "button[ng-click*='deleteDocuments']";

        public string fileNameTextbox = "//input[contains(@ng-model,'User_Defined_Filename')]";

        private string dynamicCountryProject =
            "//div[contains(@ng-if,'projectsGroup')]//span[@class='ng-binding'][text()='{0}']";

        private string dynamicCountryLegal =
            "//div[contains(@ng-if,'legalEntitiesGroup')]//span[@class='ng-binding'][text()='{0}']";

        private string dynamicLegalFA =
            "//div[contains(@ng-if,'legalEntitiesGroup')]//span[@class='ng-binding'][text()='{0}']/../../following-sibling::div//span[@class='ng-binding'][text()='{1}']";

        private string dynamicLegalFAExpandDiv =
            "//div[contains(@ng-if,'legalEntitiesGroup')]//span[@class='ng-binding'][text()='{0}']/../../following-sibling::div//span[@class='ng-binding'][text()='{1}']/../preceding-sibling::div";

        private string dynamicCountryLegalExpandDiv = "//div[contains(@ng-if,'legalEntitiesGroup')]//span[@class='ng-binding'][text()='{0}']/../preceding-sibling::div";

        private string dynamicProjectExpandDiv = "//div[contains(@ng-if,'projectsGroupCollapsed')]//span[@class='ng-binding'][text()='{0}']/../preceding-sibling::div";

        private string dynamicProjectFA =
            "//div[contains(@ng-if,'ContextLevel.Country')]//span[@class='ng-binding'][text()='{0}']/../../following-sibling::div//span[@class='ng-binding'][text()='{1}']";

        private string dynamicProjectFAExpandDiv =
           "//div[contains(@ng-if,'ContextLevel.Country')]//span[@class='ng-binding'][text()='{0}']/../../following-sibling::div//span[@class='ng-binding'][text()='{1}']/../preceding-sibling::div";

        private string dynamicProjectPhase = "//div[contains(@ng-if,'ContextLevel.Country')]//span[text()='{0}']/..";

        private string dynamicProjectPhaseExpandDiv = "//div[contains(@ng-if,'ContextLevel.Country')]//span[text()='{0}']/../preceding-sibling::div";

        private string dynamicDocumentImg = "//div[@ng-bind='item.ContentTypeName'][text()='{0}']/..";

        private string dynamicOwningEntityItem =
            "//span[@ng-bind='field.Label'][text()='Owning entity']/../following-sibling::div//span[contains(@class, 'ms-Dropdown-title')]/following-sibling::ul[@class='ms-Dropdown-items']//li[text()='{0}']";

        private string dynamicPermissionItem =
            "//span[text()='Select permission type']/../following-sibling::div//span[contains(@class, 'ms-Dropdown-title')]/following-sibling::ul[@class='ms-Dropdown-items']//li[contains(text(),'{0}')]";

        private string dynamicAutoCompleteItemLabel = "//li[contains(@class,'ms-PeoplePicker-result')]//div[@class='ms-Persona-primaryText'][text()='{0}']";
        private string dynamicDate = "//a[@ng-repeat='item in days'][text()='{0}']";

        private string dynamicGridFileName =
            "//span[contains(@ng-bind,'document.FileName')][text()='{0}' or contains(text(),'{0}')]";

        private string dynamicFileCheckbox =
            "//span[contains(@ng-bind,'document.FileName')][text()='{0}' or contains(text(),'{0}')]/../../preceding-sibling::td";
        private string documentTypeSearchTextbox = "//uif-searchbox[@ng-model='$ctrl.searchTypes.ContentTypeName']//input";

        private string filePublisherTextbox = "//span[text()='Publisher']/../following-sibling::div//input";
        private string fileMediaPublishedDateTextbox = "//span[text()='Media Published Date']/../following-sibling::div//input";
        private string fileMediaDropdown = "//span[text()='Media']/../following-sibling::div//span[contains(@class, 'ms-Dropdown-title')]";
        private string dynamicMediaItem =
            "//span[text()='Media']/../following-sibling::div//span[contains(@class, 'ms-Dropdown-title')]/following-sibling::ul[@class='ms-Dropdown-items']//li[contains(text(),'{0}')]";

        //Import dialog
        private string uploadInput = "//input[@type='file' and @ng-model='$ctrl.file']";
        private string uploadNextButton = "//button/span[contains(.,'Next')]";
        private string setDocumentPropertiesButton = "//button/span[contains(.,'Set document properties')]";
        private string importDocumentToSIMSButton = "//button/span[contains(.,'Import document to SIMS')]";
        private string publishedDateTextbox = "//input[contains(@placeholder,'Input published date')]";
        private string dynamicSourceCheckbox = "//label/span[contains(.,'{0}')]";
        private string dynamicStateCheckbox = "//label/span[contains(.,'{0}')]";
        private string dynamicCountryExpandDiv = "//div[@class='item-selector dots']//span[text()='{0}']/../preceding-sibling::div";
        private string importType = "//input[@placeholder='Please select a type']/..";
        private string dynamicTypeItem = "//div[@ng-bind-html='item.ContentTypeName' and text()='{0}']";
        private string importFileName = "//input[@ng-model='$ctrl.data.User_Defined_Filename']";

        //New message
        private string toTextbox = "input[placeholder*='Start typing the recipient']";
        private string subjectTextbox = "input[placeholder*='Enter a subject line.']";
        private string messageAreabox = "//textarea[@placeholder='Enter a message.']";
        private string sendButton = "button[ng-click='$ctrl.shareDocument()']";

        //Move file dialog
        private string moveFileButton = "//button[@ng-if='!$ctrl.disableDelete']";

        //Compare file dialog
        private string compareLeftModule = "//div[@ng-if='$ctrl.leftDisplayedImageIndex > -1' and @class='image-preview ng-scope']";
        private string compareRightModule = "//div[@ng-if='$ctrl.rightDisplayedImageIndex > -1' and @class='image-preview ng-scope']";

        //View file dialog
        private string viewModule = "//div[@ng-if='$ctrl.displayedImageIndex > -1' and @class='image-preview ng-scope']";

        public void SelectCountry(string country)
        {
            WaitForMainLoading();
            country = string.Format(dynamicCountry, country);
            browserMonkey.ScrollIntoView(country);
            browserMonkey.Click(country);
            WaitForMainLoading();
        }

        public void ExpandCountry(string country)
        {
            WaitForMainLoading();
            country = string.Format(dynamicCountryExpandDiv, country);
            browserMonkey.ScrollIntoView(country);
            browserMonkey.Click(country);
            WaitForMainLoading();
        }

        public void ExpandLegalEntity(string legalEntity)
        {
            WaitForMainLoading();
            legalEntity = string.Format(dynamicCountryLegalExpandDiv, legalEntity);
            browserMonkey.ScrollIntoView(legalEntity);
            browserMonkey.Click(legalEntity);
            WaitForMainLoading();
        }

        public void ExpandProjectPhase(string phase)
        {
            WaitForMainLoading();
            phase = string.Format(dynamicProjectPhaseExpandDiv, phase);
            browserMonkey.ScrollIntoView(phase);
            browserMonkey.Click(phase);
            WaitForMainLoading();
        }

        public void ExpandProject(string project)
        {
            WaitForMainLoading();
            project = string.Format(dynamicProjectExpandDiv, project);
            browserMonkey.ScrollIntoView(project);
            browserMonkey.Click(project);
            WaitForMainLoading();
        }

        public void ExpandLegalEntityFA(string legalEntity, string fa)
        {
            WaitForMainLoading();
            legalEntity = string.Format(dynamicLegalFAExpandDiv, legalEntity, fa);
            browserMonkey.ScrollIntoView(legalEntity);
            browserMonkey.Click(legalEntity);
            WaitForMainLoading();
        }

        public void ExpandProjectFA(string contextLevelCountry, string fa)
        {
            WaitForMainLoading();
            contextLevelCountry = string.Format(dynamicProjectFAExpandDiv, contextLevelCountry, fa);
            browserMonkey.ScrollIntoView(contextLevelCountry);
            browserMonkey.Click(contextLevelCountry);
            WaitForMainLoading();
        }

        public void SelectSourceCheckbox(string source)
        {
            source = string.Format(dynamicSourceCheckbox, source);
            browserMonkey.Click(source);
            WaitForMainLoading();
        }

        public void SelectState(string state)
        {
            state = string.Format(dynamicStateCheckbox, state);
            browserMonkey.Click(state);
            WaitForMainLoading();
        }

        public bool IsItemSelected(string country)
        {
            return browserMonkey.UnreliableElements(selectedTreeItem).Any(element => element.Text.Trim().Equals(country));
        }

        public void SelectCountryProject(string project)
        {
            string element = string.Format(dynamicCountryProject, project);
            browserMonkey.ScrollIntoView(element);
            browserMonkey.Click(element);
            WaitForMainLoading();
        }

        public void SelectProjectPhase(string phase)
        {
            string element = string.Format(dynamicProjectPhase, phase);
            browserMonkey.ScrollIntoView(element);
            browserMonkey.Click(element);
            WaitForMainLoading();
        }

        public void SelectLegalFA(string legal, string fa)
        {
            string element = string.Format(dynamicLegalFA, legal, fa);
            browserMonkey.ScrollIntoView(element);
            browserMonkey.Click(element);
            WaitForMainLoading();
        }

        public void SelectProjectFA(string contextLevelCountry, string fa)
        {
            string element = string.Format(dynamicProjectFA, contextLevelCountry, fa);
            browserMonkey.ScrollIntoView(element);
            browserMonkey.Click(element);
            WaitForMainLoading();
        }

        public void SelectCountryLegal(string entity)
        {
            string element = string.Format(dynamicCountryLegal, entity);
            browserMonkey.ScrollIntoView(element);
            browserMonkey.Click(element);
            WaitForMainLoading();
        }

        public void SelectActivity(string activity)
        {
            string element = string.Format(dynamicActivity, activity);
            browserMonkey.ScrollIntoView(element);
            browserMonkey.Click(element);
            WaitForMainLoading();
        }

        public string GetSelectedActivity()
        {
            return browserMonkey.Read(selectedActivityLabel);
        }

        public void ClickAddDocumentButton()
        {
            browserMonkey.Click(newButton);
            browserMonkey.Click(newDocumentButton);
            WaitForMainLoading();
        }

        public void ClickAddActivityButton()
        {
            browserMonkey.Click(newButton);
            browserMonkey.Click(newActivityButton);
            WaitForMainLoading();
        }

        public void SeletImportMenu(string item)
        {
            var temp = "";
            switch (item)
            {
                case "Single Document":
                    temp = "openImportDialog()";
                    break;
                case "Bulk Draft":
                    temp = "openBulkImportDialog($ctrl.Constants.Draft)";
                    break;
                case "Bulk Published":
                    temp = "openBulkImportDialog($ctrl.Constants.Published)";
                    break;
            }

            browserMonkey.Click(importButton);
            browserMonkey.Click(string.Format(dynamicMenuItem, temp));
            WaitForMainLoading();
        }

        public void ClickSetDocumentPropertiesButton()
        {
            browserMonkey.Click(setDocumentPropertiesButton);
            WaitForMainLoading();
        }

        public void ClickImportDocumentToSIMSButton()
        {
            browserMonkey.Click(importDocumentToSIMSButton);
            browserMonkey.Sleep(3);
            WaitForMainLoading();
        }

        public void ClickNextButton()
        {
            browserMonkey.WaitForControlAppear(uploadNextButton);
            browserMonkey.Click(uploadNextButton);
            WaitForMainLoading();
        }

        public string GetDialogTitle()
        {
            return browserMonkey.Read(dialogTitleLabel);
        }

        public void ClickDocumentType(string type)
        {
            browserMonkey.Write(documentTypeSearchTextbox, type);
            browserMonkey.Hover(string.Format(dynamicDocumentImg, type));
            browserMonkey.Sleep(1);
            browserMonkey.Click(string.Format(dynamicDocumentImg, type));
        }

        public void ClickProvideDetailButton()
        {
            browserMonkey.Click(provideDetailButton);
        }

        public bool IsProvideDetailButtonDisplayed()
        {
            return browserMonkey.IsVisible(provideDetailButton);
        }

        public string GetSelectedStep()
        {
            return browserMonkey.Read(selectedStepLabel).Trim();
        }

        public void EnterFileInfo(FileInfo fileInfo)
        {
            if (!string.IsNullOrEmpty(fileInfo.FileName))
            {
                browserMonkey.Write(fileNameTextbox, fileInfo.FileName);
            }

            if (!string.IsNullOrEmpty(fileInfo.FileTitle))
            {
                browserMonkey.Write(fileTitleTextbox, fileInfo.FileTitle);
            }

            if (!string.IsNullOrEmpty(fileInfo.FileDescription))
            {
                browserMonkey.Write(fileDescriptionAreabox, fileInfo.FileDescription);
            }

            if (!string.IsNullOrEmpty(fileInfo.Owner))
            {
                browserMonkey.Sleep(1);
                browserMonkey.Write(fileOwnerTextbox, fileInfo.Owner);
                //browserMonkey.Enter(fileOwnerTextbox);
                browserMonkey.Click(string.Format(dynamicAutoCompleteItemLabel, fileInfo.Owner));
            }

            if (!string.IsNullOrEmpty(fileInfo.OwningEntity))
            {
                browserMonkey.Click(fileOwningEntityDropdown);
                browserMonkey.Click(string.Format(dynamicOwningEntityItem, fileInfo.OwningEntity));
            }

            if (!string.IsNullOrEmpty(fileInfo.Author))
            {
                browserMonkey.Sleep(1);
                browserMonkey.Write(fileAuthorTextbox, fileInfo.Author);
                //browserMonkey.Enter(fileAuthorTextbox);
                browserMonkey.Click(string.Format(dynamicAutoCompleteItemLabel, fileInfo.Author));
            }

            if (!string.IsNullOrEmpty(fileInfo.MeetingDateAndTime))
            {
                browserMonkey.Click(fileMeetingDateAndTimeTextbox);
                browserMonkey.Click(string.Format(dynamicDate, fileInfo.MeetingDateAndTime));
            }

            if (!string.IsNullOrEmpty(fileInfo.EffectiveDate))
            {
                browserMonkey.Click(fileEffectiveDateTextbox);
                browserMonkey.Click(string.Format(dynamicDate, fileInfo.EffectiveDate));
            }

            if (!string.IsNullOrEmpty(fileInfo.Publisher))
            {
                browserMonkey.Write(filePublisherTextbox, fileInfo.Publisher);
            }

            if (!string.IsNullOrEmpty(fileInfo.MediaPublishedDate))
            {
                browserMonkey.Click(fileMediaPublishedDateTextbox);
                browserMonkey.Click(string.Format(dynamicDate, fileInfo.MediaPublishedDate));
            }

            if (!string.IsNullOrEmpty(fileInfo.Media))
            {
                browserMonkey.Sleep(1);
                browserMonkey.Click(fileMediaDropdown);
                browserMonkey.Click(string.Format(dynamicMediaItem, fileInfo.Media));
            }

            if (!string.IsNullOrEmpty(fileInfo.PublishedDate))
            {
                browserMonkey.Click(publishedDateTextbox);
                browserMonkey.Click(string.Format(dynamicDate, fileInfo.PublishedDate));
            }

            if (!string.IsNullOrEmpty(fileInfo.Person))
            {
                browserMonkey.Write(filePersonTextbox, fileInfo.Person);
                browserMonkey.Enter(filePersonTextbox);
                browserMonkey.Click(string.Format(dynamicAutoCompleteItemLabel, fileInfo.Person));
            }

            if (!string.IsNullOrEmpty(fileInfo.Permission))
            {
                browserMonkey.Sleep(1);
                browserMonkey.Click(filePermissionDropdown);
                browserMonkey.Click(string.Format(dynamicPermissionItem, fileInfo.Permission));
            }
        }

        public void ClickCreateDocumentButton(bool waitToastMessage = false)
        {
            WaitForMainLoading();
            browserMonkey.ScrollIntoView(createDocumentButton);
            browserMonkey.Click(createDocumentButton);
            WaitForMainLoading();
            WaitForCreating();
            if (waitToastMessage)
            {
                WaifForToastMessageAppearCompletely(documentWillCreate);
            }
        }

        public void ClickCreateCopyButton()
        {
            browserMonkey.Click(createCopyButton);
            WaitForMainLoading();
        }

        public bool DoesFileExist(string fileName)
        {
            browserMonkey.WaitForControlAppear(string.Format(dynamicGridFileName, fileName));
            return browserMonkey.IsVisible(string.Format(dynamicGridFileName, fileName));
        }

        public bool DoesFileNotExist(string fileName)
        {
            browserMonkey.WaitForControlDisappear(string.Format(dynamicGridFileName, fileName));
            return !browserMonkey.IsVisible(string.Format(dynamicGridFileName, fileName));
        }

        public void DeleteFile(string fileName, string reason)
        {
            SelectFile(fileName);
            browserMonkey.Click(deleteButton);
            WaitForMainLoading();
            browserMonkey.Write(deleteReasonTextbox, reason);
            browserMonkey.Click(deleteDocumentButton);
            WaitForMainLoading();
        }

        public void SelectFile(string fileName)
        {
            browserMonkey.TryWaitForJavascriptIdle();
            UnselectAllFiles();
            browserMonkey.Click(string.Format(dynamicFileCheckbox, fileName));
        }

        public void UnselectAllFiles()
        {
            browserMonkey.Click(selectAllFiles);
            browserMonkey.Click(selectAllFiles);
        }

        public void DeleteAllFiles(string reason)
        {
            browserMonkey.Click(selectAllFiles);
            browserMonkey.Click(deleteButton);
            WaitForMainLoading();
            browserMonkey.Write(deleteReasonTextbox, reason);
            browserMonkey.Click(deleteDocumentButton);
            WaitForMainLoading();
            //browserMonkey.WaitForControlDisappear(string.Format(dynamicGridFileName, fileName));
        }

        public void UploadFile(string filePath)
        {
            browserMonkey.JavaScriptSetAttribute(uploadInput + "/..", "style", "isibility: visible;position: absolute;width: 100px;height: 100px;border: none;margin-top: 200px;padding: 0px;");
            browserMonkey.Sleep(1);
            browserMonkey.Write(uploadInput, filePath);
            browserMonkey.Sleep(1);
            WaitForMainLoading();
        }

        public void SelectShareItems(string item)
        {
            browserMonkey.Click(shareButton);
            browserMonkey.Click(string.Format(dynamicShareItems, item));
        }

        public void SelectMenuItems(string item)
        {
            var temp = "";
            switch (item)
            {
                case "Move":
                    temp = "moveDocument";
                    break;
                case "Download":
                    temp = "download";
                    break;
                case "Rename":
                    temp = "renameDocument";
                    break;
                case "Edit Document":
                    temp = "editDocument";
                    break;
                case "About File":
                    temp = "showProperties";
                    break;
                case "View Audit Log":
                    temp = "viewAuditLog";
                    break;
            }

            browserMonkey.Click(menuButton);
            browserMonkey.Click(string.Format(dynamicMenuItem, temp));
            WaitForMainLoading();
        }

        public void SendNewMessage(string to, string subject, string message)
        {
            while (!browserMonkey.Exists(".tag-item.ng-scope"))
            {
                browserMonkey.Write(toTextbox, to);
                browserMonkey.Sleep(1);
                browserMonkey.Enter(toTextbox);
            }
            browserMonkey.Write(subjectTextbox, subject);
            browserMonkey.Write(messageAreabox, message);
            browserMonkey.Click(sendButton);
            WaifForToastMessageAppearCompletely(processStarted);
        }

        public bool DoesLegalFAActivityExist(string legal, string activity)
        {
            string element = string.Format(dynamicLegalFA, legal, activity);
            browserMonkey.ScrollIntoView(element);
            return browserMonkey.IsVisible(element);
        }

        public bool DoesProjectFAActivityExist(string contextLevelCountry, string activity)
        {
            string element = string.Format(dynamicProjectFA, contextLevelCountry, activity);
            browserMonkey.ScrollIntoView(element);
            return browserMonkey.IsVisible(element);
        }

        public void RenameFile(string fileName, string newFileName)
        {
            SelectFile(fileName);
            SelectMenuItems("Rename");
            browserMonkey.Write(renameTextbox, newFileName);
            browserMonkey.Click(saveRenameButton);
            WaifForToastMessageAppearCompletely(documentWillRename);
        }

        public List<string> CreateDocuments(FileInfo fileInfo, string documentType, int quantity)
        {
            List<string> fileNameList = new List<string>();

            for (int i = 0; i < quantity; i++)
            {
                ClickAddDocumentButton();
                ClickDocumentType(documentType);
                fileInfo.FileName = "AUTO" + Guid.NewGuid().ToString().Split('-')[0];
                EnterFileInfo(fileInfo);
                ClickCreateDocumentButton();

                fileNameList.Add(fileInfo.FileName);
            }

            return fileNameList;
        }

        public List<string> CreateDocuments(FileInfo fileInfo, string documentType, int quantity, bool wait = true)
        {
            List<string> fileNameList = new List<string>();

            for (int i = 0; i < quantity; i++)
            {
                ClickAddDocumentButton();
                ClickDocumentType(documentType);
                fileInfo.FileName = "AUTO" + Guid.NewGuid().ToString().Split('-')[0];
                EnterFileInfo(fileInfo);
                ClickCreateDocumentButton(wait);

                fileNameList.Add(fileInfo.FileName);
            }

            return fileNameList;
        }

        public bool DoesFilesExist(List<string> fileNameList, string prefix, string postfix)
        {
            foreach (string name in fileNameList)
            {
                string fileName = prefix + name + postfix;
                browserMonkey.WaitForControlAppear(string.Format(dynamicGridFileName, fileName));
                if (browserMonkey.IsVisible(string.Format(dynamicGridFileName, fileName)).Equals(false))
                {
                    return false;
                }
            }

            return true;
        }

        public void MoveFile(string fileName, string newPath)
        {
            string country = newPath.Split('>')[0];
            string project = newPath.Split('>')[1];
            SelectFile(fileName);
            SelectMenuItems("Move");
            ExpandCountry(country);
            SelectCountryProject(project);
            browserMonkey.Click(moveFileButton);
            WaitForMainLoading();
        }

        public void EditTitleAboutFile(string fileName, string title, string description)
        {
            SelectFile(fileName);
            SelectMenuItems("About File");
            browserMonkey.Write(aboutFileTitleTextbox, title);
            browserMonkey.Write(aboutFileDescriptionTextarea, description);
            browserMonkey.Click(aboutFileSaveButton);
            WaifForToastMessageAppearCompletely(documentWillUpdate);
        }

        public bool DoesAboutFileCorrect(string fileName, string title, string description)
        {
            SelectFile(fileName);
            SelectMenuItems("About File");
            browserMonkey.ScrollIntoView(aboutFileTitleTextbox);
            string titleActual = browserMonkey.GetAttribute(aboutFileTitleTextbox, "value");
            string descriptionActual = browserMonkey.GetAttribute(aboutFileDescriptionTextarea, "value");

            if (titleActual.Equals(title) && descriptionActual.Equals(description))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CloseAboutFile()
        {
            browserMonkey.Click(aboutFileCancelButton);
        }
        
        public void FillProvideDetailsImportFile(ImportFileInfo importFileInfo)
        {
            if (!string.IsNullOrEmpty(importFileInfo.Type))
            {
                browserMonkey.Click(importType);
                browserMonkey.Click(string.Format(dynamicTypeItem, importFileInfo.Type));
            }

            if (!string.IsNullOrEmpty(importFileInfo.Source))
            {
                SelectSourceCheckbox(importFileInfo.Source);
            }

            if (!string.IsNullOrEmpty(importFileInfo.State))
            {
                SelectState(importFileInfo.State);
            }
        }

        public void ImportFile(string filePath, ImportFileInfo importFileInfo, FileInfo fileInfo)
        {
            UploadFile(filePath);
            ClickNextButton();
            FillProvideDetailsImportFile(importFileInfo);
            EnterFileInfo(fileInfo);
            ClickImportDocumentToSIMSButton();
            WaitForCreating();
        }

        public void CompareFile(string fileName1, string fileName2)
        {
            browserMonkey.Click(string.Format(dynamicFileCheckbox, fileName1));
            browserMonkey.Click(string.Format(dynamicFileCheckbox, fileName2));
            browserMonkey.Click(compareButton);
            WaitForMainLoading();
        }

        public bool DoesCompareModuleOpen()
        {
            return (browserMonkey.IsVisible(compareLeftModule) && browserMonkey.IsVisible(compareRightModule));
        }

        public void CloseModule()
        {
            browserMonkey.Click(dialogCloseButton);
        }

        public void ViewFile(string fileName)
        {
            SelectFile(fileName);
            browserMonkey.Click(viewButton);
            WaitForMainLoading();
        }

        public bool DoesViewModuleOpen()
        {
            return browserMonkey.IsVisible(viewModule);
        }
    }
}

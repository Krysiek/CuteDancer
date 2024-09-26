using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VRF
{
    public class MainViewEditor : VisualElement
    {
        public enum ViewState
        {
            Builder = 0,
            Installer = 1,
            About = 2
        }

        private readonly Button builderBtn;
        private readonly Button installerBtn;
        private readonly Button aboutBtn;

        private readonly VisualElement viewContent;

        private readonly Button[] buttons;
        private readonly VisualElement[] views;

        private ViewState currentView = ViewState.Builder;

        public MainViewEditor()
        {
            CuteResources.LoadView("MainView").CloneTree(this);
            builderBtn = this.Q<Button>("BuilderBtn");
            installerBtn = this.Q<Button>("InstallerBtn");
            aboutBtn = this.Q<Button>("AboutBtn");

            List<Button> buttonsList = new List<Button>
            {
                builderBtn,
                installerBtn,
                aboutBtn
            };
            buttons = buttonsList.ToArray();

            List<VisualElement> viewsList = new List<VisualElement>
            {
                new BuilderViewEditor(),
                new InstallerViewEditor(),
                new AboutView()
            };
            views = viewsList.ToArray();

            viewContent = this.Q<VisualElement>("ViewContent");

            builderBtn.clickable = new Clickable(() => SelectView(ViewState.Builder));
            installerBtn.clickable = new Clickable(() => SelectView(ViewState.Installer));
            aboutBtn.clickable = new Clickable(() => SelectView(ViewState.About));

            SelectView(ViewState.Builder);
        }

        public void SelectView(ViewState windowState)
        {
            currentView = windowState;
            UpdateButtonsClasses();
            UpdateViewContent();
        }

        private void UpdateViewContent()
        {
            viewContent.Clear();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (((int)currentView) == i)
                {
                    viewContent.Add(views[i]);
                    return;
                }
            }
        }

        public void UpdateButtonsClasses()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (((int)currentView) == i)
                {
                    buttons[i].AddToClassList("banner-button-active");
                }
                else
                {
                    buttons[i].RemoveFromClassList("banner-button-active");
                }
            }
        }

    }
}

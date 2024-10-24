﻿namespace Lab
{
    public partial class Page2 : ContentPage
    {
        int count = 0;

        public Page2() => InitializeComponent();

        private void OnCounterClicked(object sender, EventArgs e)
        {

            count++;

            if (count == 1)
                NewBtn.Text = $"Clicked {count} time";
            else
                NewBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(NewBtn.Text);
        }
    }

}

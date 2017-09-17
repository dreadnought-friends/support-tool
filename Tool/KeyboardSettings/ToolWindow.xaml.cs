using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SupportTool.Tool.KeyboardSettings
{
    public partial class ToolWindow : Window
    {
        private LoggerInterface Logger;
        private ModulePreset Preset;

        public ToolWindow(ModulePreset modulePreset, LoggerInterface logger)
        {
            Preset = modulePreset;
            Logger = logger;

            InitializeComponent();

            ModulePresetOptions.SelectionChanged += new SelectionChangedEventHandler(delegate (object sender, SelectionChangedEventArgs e)
            {
                ComboBox comboBox = (ComboBox)sender;
                
                if (-1 == comboBox.SelectedIndex || comboBox.SelectedIndex > Preset.Presets.Count)
                {
                    return;
                }

                if (comboBox.SelectedIndex == Preset.Presets.Count)
                {
                    ShowShortcutPreview(Preset.ActiveConfiguration);
                    SaveChanges.IsEnabled = false;
                    return;
                }

                ShowShortcutPreview(Preset.Presets[comboBox.SelectedIndex]);
                SaveChanges.IsEnabled = true;
            });
        }

        public void InitializeKeybindings()
        {
            bool selected = false;
            SaveChanges.IsEnabled = false;
            ModulePresetOptions.IsEnabled = false;
            Tip.Visibility = Visibility.Collapsed;
            Error.Visibility = Visibility.Collapsed;

            ModulePresetConfiguration current = Preset.ActiveConfiguration;

            if (null == current)
            {
                Error.Visibility = Visibility.Visible;
                return;
            }

            Tip.Visibility = Visibility.Visible;

            ModulePresetOptions.Items.Clear();

            foreach (ModulePresetConfiguration preset in Preset.Presets)
            {
                if (preset == current)
                {
                    selected = true;
                }

                ModulePresetOptions.Items.Add(new ComboBoxItem()
                {
                    Content = preset.Name,
                    IsSelected = preset == current,
                });
            }

            ModulePresetOptions.Items.Add(new ComboBoxItem()
            {
                Content = current.Name,
                IsSelected = !selected,
                IsEnabled = false,
            });

            List<ModulePresetConfiguration> presets = new List<ModulePresetConfiguration>(Preset.Presets) { current };
            
            ShowShortcutPreview(presets[ModulePresetOptions.SelectedIndex]);

            SaveChanges.IsEnabled = current != presets[ModulePresetOptions.SelectedIndex];
            ModulePresetOptions.IsEnabled = true;
        }

        private void ShowShortcutPreview(ModulePresetConfiguration preset)
        {
            PrimaryModule.Text = preset.Primary;
            SecondaryModule.Text = preset.Secondary;
            PerimeterModule.Text = preset.Perimeter;
            InternalModule.Text = preset.Internal;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            ModulePresetConfiguration preset = Preset.Presets[ModulePresetOptions.SelectedIndex];

            Preset.savePreset(preset);

            Logger.Log(String.Format("Module preset set to {0}", preset.Name));

            Hide();
        }
    }
}

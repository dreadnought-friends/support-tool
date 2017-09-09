using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SupportTool
{
    class CommandContainer
    {
        private Config Config;
        private Panel ParentUiElement;
        private List<Tuple<CommandInterface, CheckBox>> CommandTuple = new List<Tuple<CommandInterface, CheckBox>>();

        /// <summary>
        /// All Commands attached, regardless of CheckBox.
        /// </summary>
        public List<CommandInterface> Commands
        {
            get
            {
                return CommandTuple.Select(item => item.Item1).ToList();
            }

            private set { }
        }

        /// <summary>
        /// All CheckBoxes created for the Commands.
        /// </summary>
        public List<CheckBox> CheckBoxes
        {
            get
            {
                return CommandTuple.Select(item => item.Item2).Where(checkbox => null != checkbox).ToList();
            }

            private set { }
        }

        public CommandContainer(Config config, Panel parentUiElement)
        {
            Config = config;
            ParentUiElement = parentUiElement;
        }

        /// <summary>
        /// Enables all checkboxes.
        /// </summary>
        public void Enable()
        {
            foreach (CheckBox checkBox in CheckBoxes)
            {
                checkBox.IsEnabled = true;
            }
        }

        /// <summary>
        /// Disables all checkboxes
        /// </summary>
        public void Disable()
        {
            foreach (CheckBox checkBox in CheckBoxes)
            {
                checkBox.IsEnabled = false;
            }
        }

        /// <summary>
        /// Adds a command to the set.
        /// 
        /// Whenever the CommandCheckBoxInterface is implemented, a checkbox
        /// will be added to the parent UI element.
        /// </summary>
        /// <param name="command"></param>
        public void Add(CommandInterface command)
        {
            if (false == command is CommandCheckBoxInterface)
            {
                // no checkbox has to be created and kept track of, return early
                CommandTuple.Add(Tuple.Create<CommandInterface, CheckBox>(command, null));
                return;
            }

            CommandCheckBoxInterface elementAware = (CommandCheckBoxInterface)command;

            CheckBox uiElement = new CheckBox()
            {
                Margin = new Thickness(3, 3, 0, 0),
                Content = elementAware.Text
            };

            if (null != elementAware.ToolTip)
            {
                uiElement.ToolTip = new ToolTip()
                {
                    Content = new TextBlock()
                    {
                        Text = elementAware.ToolTip,
                        MaxWidth = 300,
                    }
                };
            }

            // creates a 2 way binding between the config values and the element
            Binding binding = new Binding();
            binding.Source = Config;
            binding.Path = new PropertyPath(elementAware.ConfigPropertyPath);
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(uiElement, CheckBox.IsCheckedProperty, binding);

            ParentUiElement.Children.Add(uiElement);

            CommandTuple.Add(Tuple.Create<CommandInterface, CheckBox>(command, uiElement));
        }
    }
}

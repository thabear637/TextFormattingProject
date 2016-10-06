
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TextFormattingProject
{
    public class MainViewModel : ViewModelBase
    {
  
        #region Commands
        private RelayCommand<FlowDocument> _TextInputCommand;
        public ICommand TextInputCommand
        {
            get
            {
                if (_TextInputCommand == null)
                    _TextInputCommand = new RelayCommand<FlowDocument>((doc) => FormatText(doc), (doc) => CanFormatText);
                return _TextInputCommand;
            }
        }
        
        #endregion

        #region Methods

        private void FormatText(FlowDocument doc)
        {
            //Only run if text has actually changed
            if (IsChanging)
                return;
            IsChanging = true;


            TextRange FullRange = new TextRange(doc.ContentStart, doc.ContentEnd);
            string text = FullRange.Text;
            
            //I'm not the best with Regex - But this I believe works
            Regex rgx = new Regex(@"([@#])\w+\s?");
            var Matches = rgx.Matches(text);
            
            //Seems whenever I make a change, the pointers get messed up.  This will try to keep even with it.
            int multiplier = 2;
            foreach (Match match in Matches)
            {
                TextPointer start = doc.ContentStart.GetPositionAtOffset(match.Index + multiplier);
                TextPointer end = doc.ContentStart.GetPositionAtOffset(match.Index + match.Length + multiplier);


                TextRange Range = new TextRange(start, end);
                Range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);


                //switch back to black text at the end
                if (match == Matches[Matches.Count - 1])
                {
                    TextPointer next = end;
                    TextRange endRange = new TextRange(next, doc.ContentEnd);
                    endRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                }

                //increment multiplier - have tried 1, 2, 3, 4 and 2 seems to get the closest, but it's not perfect
                multiplier += 2; 
                
            }

            IsChanging = false;
        }
        
        #endregion

        #region Properties

        //allow/disallow Formatting of text
        public bool CanFormatText { get { return !IsChanging; } }

        private bool IsChanging = false;


        #endregion
    }
}

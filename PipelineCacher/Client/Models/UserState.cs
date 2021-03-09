using PipelineCacher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Models
{
    public class UserState
    {
        private string test = "This is before logging in";

        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public string Test { 
            get => test; 
            set {
                test = value;
                NotifyStateChanged();
            }
        }
        public User User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AI4Good.Renderers
{
    public interface IAudio
    {
        bool PlayBase64(string content);
        bool IsPlaying { get; set; }
        event EventHandler StateChanged;
    }
}

using Ai4Good_ConversationalAi.Common.Interfaces;
using Ai4Good_ConversationalAi.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Implementation.Repositories
{
    public class TranscriptRepository : ITranscriptRepository
    {
        private List<SpeechTranscript> _SpeechTranscript = new List<SpeechTranscript>();
        private bool disposedValue;

        public int LogTask(SpeechTranscript transcript)
        {
            try
            {
                _SpeechTranscript.Add(transcript);
            }
            catch (Exception)
            {
                return 0;
            }
            return 1;
        }

        Task<int> ITranscriptRepository.LogTask(SpeechTranscript status)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _SpeechTranscript = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TranscriptRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}

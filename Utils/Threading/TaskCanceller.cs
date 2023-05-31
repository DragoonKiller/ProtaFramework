using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Prota
{
    
    public class TaskCanceller
    {
        public struct Token
        {
            public readonly CancellationTokenSource src;
            public bool cancelled => isNone ? false : src.IsCancellationRequested;
            public bool isNone => src == null;
            public Token(TaskCanceller cc) => this.src = cc.currentSource;
        }
        
        public Token token => new Token(this);
        
        public bool Valid(Token token) => token.src == currentSource;
        
        CancellationTokenSource currentSource;
        
        public bool isSetup => currentSource != null;
        
        
        public Token Setup()
        {
            Cancel();
            currentSource = new CancellationTokenSource();
            return token;
        }
        
        public void Cancel()
        {
            currentSource?.Cancel();
            currentSource = null;
        }
    }
    
    public static partial class MethodExtensions
    {
        public static bool IsCancelled(this TaskCanceller.Token? token)
            => token == null ? false : token.Value.cancelled;
    }
}

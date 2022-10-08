using System;

namespace gerencianet.PIX
{
    public class PixResult
    {
        public Exception error;
        public object data;

        public PixResult() { }
        public PixResult(object data)
        {
            this.data = data;

        }
        public PixResult(Exception error)
        {
            this.error = error;
        }
    }
}
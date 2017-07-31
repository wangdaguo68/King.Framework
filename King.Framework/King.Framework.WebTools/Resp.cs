
﻿namespace King.Framework.WebTools
{
    using System;
    using System.Runtime.CompilerServices;

    public class Resp
    {
        private int _errorCode = 0;
        private bool _state = true;

        public static Resp BusinessError(string errorMessage)
        {
            return new Resp { State = false, ErrorCode = -1, ErrorMessage = errorMessage };
        }

        public static Resp BusinessError(int errorCode, string errorMessage)
        {
            return new Resp { State = false, ErrorCode = errorCode, ErrorMessage = errorMessage };
        }

        public static Resp<T> BusinessError<T>(string errorMessage, T data)
        {
            return new Resp<T> { State = false, ErrorCode = -1, ErrorMessage = errorMessage, Data = data };
        }

        public static Resp ServerError()
        {
            return new Resp { State = false, ErrorCode = -1, ErrorMessage = "服务器执行异常" };
        }

        public static Resp Success()
        {
            return new Resp { State = true, ErrorCode = 0, ErrorMessage = null };
        }

        public static Resp<T> Success<T>(T obj)
        {
            return new Resp<T> { State = true, ErrorCode = 0, ErrorMessage = null, Data = obj };
        }

        public int ErrorCode
        {
            get
            {
                return this._errorCode;
            }
            set
            {
                this._errorCode = value;
            }
        }

        public string ErrorMessage { get; set; }

        public bool State
        {
            get
            {
                return this._state;
            }
            set
            {
                if (value)
                {
                    this._errorCode = 0;
                }
                this._state = value;
            }
        }
    }
}


﻿namespace Todo.Application
{
    public struct Unit
    {
        public static readonly Unit Value = new Unit();

        public override string ToString() => "()";
    }
}

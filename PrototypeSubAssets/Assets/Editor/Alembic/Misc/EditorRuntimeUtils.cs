using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorRuntimeUtils : MonoBehaviour
{
    internal class DisableUndoGuard : IDisposable
    {
        internal static bool enableUndo = true;
        static readonly Stack<bool> m_UndoStateStack = new Stack<bool>();
        bool m_Disposed;
        public DisableUndoGuard(bool disable)
        {
            m_Disposed = false;
            m_UndoStateStack.Push(enableUndo);
            enableUndo = !disable;
        }

        public void Dispose()
        {
            if (!m_Disposed)
            {
                if (m_UndoStateStack.Count == 0)
                {
                    Debug.LogError("UnMatched DisableUndoGuard calls");
                    enableUndo = true;
                    return;
                }
                enableUndo = m_UndoStateStack.Pop();
                m_Disposed = true;
            }
        }
    }
}

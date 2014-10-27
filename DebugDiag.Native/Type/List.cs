﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    public class List : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::list<(.*),std::allocator<.*> >$");
        // Keep a cached copy of the instances to avoid constantly querying the dump file.
        private readonly List<NativeType> _elements = new List<NativeType>();
        private bool _built;
        private NativeType _head;

        #region Constructor
        protected override NativeInstance DeepCopy()
        {
            return new List(this);
        }

        protected List(List other)
            : base(other)
        {
            _elements = other._elements;
        }

        public List(string typename)
            : base(typename)
        {

        }
        #endregion

        #region Type Implementation
        internal override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 2, "List expects only one group");

            // Recursively parse the type of elements inside the list.
            ElementType = TypeParser.Parse(match.Groups[1].Value);
        }

        protected override void Rebase()
        {
            base.Rebase(); // Let NativeType identify the list's members.
            Size = GetIntValue("_Mysize");
            _head = GetField("_Myhead");
        }

        public override IEnumerator<NativeType> GetEnumerator()
        {
            if (_built)
            {
                foreach (var e in _elements) yield return e;
                yield break;
            }

            dynamic cur = _head.GetField("_Next");
            ulong idx = 0;
            
            // Traverse the linked list.
            while (idx < Size)
            {
                idx++;
                NativeType e = cur._Myval;
                cur = cur._Next;
                _elements.Add(e);
                yield return e;
            }
            _built = true;
        }
        #endregion
    }
}
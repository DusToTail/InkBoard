using System;
using System.Collections.Generic;

public class RequestHandler<T> where T : class
{
    public RequestHandler()
    {
        m_RequestList = new List<Request>();
    }
    ~RequestHandler()
    {
    }

    public void ProcessRequests(bool clearOnFinished)
    {
        for(int i = 0; i < m_RequestList.Count; i++)
        {
            Request request = m_RequestList[i];
            request.Execute();
        }
        if (clearOnFinished)
            m_RequestList.Clear();
    }
    public void AddNewRequest(T target, Func<T, bool> action, Func<T, bool> validCheck)
    {
        var newRequest = new Request(target, action, validCheck, null);
        m_RequestList.Add(newRequest);
    }
    public void StackNewRequestAt(T target, Func<T, bool> action, Func<T, bool> validCheck, bool addNewIfNull = true)
    {
        var baseRequest = GetRequestAt(target);
        if(baseRequest != null) 
        {
            baseRequest.SetFinalRequest(action, validCheck);
            return; 
        }
        if (addNewIfNull)
            AddNewRequest(target, action, validCheck);
    }
    public void StackNewRequestAt(int index, Func<T, bool> action, Func<T, bool> validCheck, bool addNewIfNull = true)
    {

        var baseRequest = GetRequestAt(index);
        if (baseRequest != null)
        {
            baseRequest.SetFinalRequest(action, validCheck);
            return;
        }
    }
    public void StackNewRequestAt(Func<T, bool> targetAction, Func<T, bool> action, Func<T, bool> validCheck, bool addNewIfNull = true)
    {

        var baseRequest = GetRequestAt(targetAction);
        if (baseRequest != null)
        {
            baseRequest.SetFinalRequest(action, validCheck);
        }
    }
    private Request GetRequestAt(T target)
    {
        return m_RequestList.Find(x => x.Target == target);
    }
    private Request GetRequestAt(int index)
    {
        if(index < 0 || index >= m_RequestList.Count) { return null; }
        return m_RequestList[index];
    }
    private Request GetRequestAt(Predicate<Request> predicate)
    {
        return m_RequestList.Find(predicate);
    }
    private Request GetRequestAt(Func<T, bool> targetAction)
    {
        return m_RequestList.Find(x=>x.Action == targetAction);
    }
    private Request[] GetRequests(Predicate<Request> predicate)
    {
        return m_RequestList.FindAll(predicate).ToArray();
    }


    private List<Request> m_RequestList;

    private class Request
    {
        public Request(T target, Func<T, bool> action, Func<T, bool> valid, Request ifInvalidRequest = null)
        {
            m_Target = target;
            m_Action = action;
            m_ValidCheck = valid;
            m_IfInvalidRequest = ifInvalidRequest;
        }
        ~Request()
        {
        }

        public void Execute()
        {
            Func<T, bool> finalAction = GetFinalValidAction();
            if(finalAction == null) { return; }

            var finalTarget = GetFinalRequest().Target;
            finalAction.Invoke(finalTarget);
        }

        public Func<T, bool> GetFinalValidAction()
        {
            if (!IsValid)
            {
                if (m_IfInvalidRequest != null)
                    return m_IfInvalidRequest.GetFinalValidAction();
                return null;
            }
            return m_Action;
        }

        public void SetFinalRequest(Func<T, bool> action, Func<T, bool> valid)
        {
            var final = GetFinalRequest();
            final.m_IfInvalidRequest = new Request(final.Target, action, valid, null);
        }

        public Request GetFinalRequest()
        {
            if (m_IfInvalidRequest != null)
                return m_IfInvalidRequest.GetFinalRequest();
            return this;
        }

        public Func<T, bool> Action { get { return m_Action; } }
        public bool IsValid { get { return m_ValidCheck(m_Target); } }
        public T Target { get { return m_Target; } }

        private Func<T, bool> m_Action;
        private Request m_IfInvalidRequest;
        private Func<T, bool> m_ValidCheck;
        private T m_Target;
    }
}




using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
    public class UIButton : Button
    {

        [Serializable]
        public class PersistentEvent
        {
            public string methodName;
            public Object targetObject;
        }

        public List<PersistentEvent> persistentEvents = new List<PersistentEvent>();
        
        protected override void Start()
        {
            base.Start();
            
            var events = onClick.GetPersistentEventCount();
            for (int i = 0; i < events; i++)
            {
                var methodName = onClick.GetPersistentMethodName(i);
                Object targetObject = onClick.GetPersistentTarget(i);
                persistentEvents.Add(new PersistentEvent{methodName = methodName, targetObject = targetObject});
            }
        }

        public void AddPersistentListener(string methodName, Object targetObject)
        {
            // Get the MethodInfo for the target method
            MethodInfo methodInfo = UnityEventBase.GetValidMethodInfo(
                targetObject, methodName, Type.EmptyTypes); // Assuming no arguments for simplicity

            if (methodInfo != null)
            {
                // Wrap invocation in a UnityAction that supplies default arguments if the target method expects parameters.
                var parameters = methodInfo.GetParameters();

                UnityAction action = () =>
                {
                    object[] args = new object[parameters.Length];
                    for (int p = 0; p < parameters.Length; p++)
                    {
                        var t = parameters[p].ParameterType;
                        // Use default value for value types, null for reference types.
                        args[p] = t.IsValueType ? Activator.CreateInstance(t) : null;
                    }

                    methodInfo.Invoke(targetObject, args);
                };

                onClick.AddListener(action);
            }
            
            foreach (var persistentEvent in persistentEvents)
            {
                if (persistentEvent.methodName == methodName)
                    return;
            }
            persistentEvents.Add(new PersistentEvent{methodName = methodName,targetObject = targetObject});
        }

        private Dictionary<string, bool> methodsFound = new Dictionary<string, bool>();

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            foreach (var persistentEvent in persistentEvents)
            {
                AddPersistentListener(persistentEvent.methodName,persistentEvent.targetObject);
            }
        }

#endif
        private void Update()
        {
            
        }

        public void UIBTN_OnClick()
        {

        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            SoundManager.Instance.PlaySound(tag);
        }
    }
}
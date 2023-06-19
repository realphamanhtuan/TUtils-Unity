using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
namespace TUtils{
    public static class Sandbox{
        public static bool Execute(Action action){
            if (action == null){
                Tebug.Error("TUtils Sandbox: Action null");
                return false;
            } else
                try{
                    action.Invoke();
                    return true;
                } catch (Exception e){
                    Tebug.Error("TUtils Sandbox:", e.Message, "Stacktrace:", e.StackTrace);
                    return false;
                }
        }
    }
}
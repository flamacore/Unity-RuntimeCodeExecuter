using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using UnityEngine;
using CSharpCompiler;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Executer : MonoBehaviour
{
	private InputField theCode;
	private InputField reference;
	private InputField theGameObject;
	private Toggle theMethod1;
	private Toggle theMethod2;
	GameObject goj;
	int ti=0;
	CSharpCompiler.CodeCompiler provider = new CSharpCompiler.CodeCompiler();
	CompilerParameters param = new CompilerParameters();
	void Start()
	{
		theCode = GameObject.Find ("Code Field").GetComponent<InputField> ();
		reference = GameObject.Find ("Script").GetComponent<InputField> ();
		theGameObject = GameObject.Find ("GameObject To Be Found").GetComponent<InputField> ();
		theMethod1 = GameObject.Find ("GetComponent").GetComponent<Toggle> ();
		theMethod2 = GameObject.Find ("Static").GetComponent<Toggle> ();
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			param.ReferencedAssemblies.Add(assembly.Location);
		}
	}
	public void ExecutCode()
	{
		ti++;
		string tx = theCode.text;
		if (String.IsNullOrEmpty(reference.text)) 
		{
			tx = theCode.text;
		} else {
			if (theMethod1.isOn) 
			{
				goj = GameObject.Find (theGameObject.text);
				tx = "GameObject.Find (\""+theGameObject.text+"\").GetComponent<" + reference.text + ">()." + theCode.text;
			}
			if (theMethod2.isOn) 
			{
				tx = reference.text + "." + theCode.text;
			}

		}


		param.GenerateExecutable = false;
		param.GenerateInMemory = true;
		var assembly2 = Compile (@"using UnityEngine;public class Test"+ti+"{public static void Foo"+ti+"(){" + tx + "}}");
		var method = assembly2.GetType("Test"+ti).GetMethod ("Foo"+ti);
		var del = (Action)Delegate.CreateDelegate (typeof(Action), method);
		del.Invoke ();

	}
	void LateUpdate()
	{
		if (!String.IsNullOrEmpty (theGameObject.text)) {
			theMethod1.isOn = true;
		} else {
			theMethod1.isOn = false;
		}
	}
	public Assembly Compile(string source)
	{
		
		var result = provider.CompileAssemblyFromSource(param, source);
		if (result.Errors.Count > 0) {
			var msg = new StringBuilder();
			foreach (CompilerError error in result.Errors) {
				msg.AppendFormat("Error ({0}): {1}\n",
					error.ErrorNumber, error.ErrorText);
			}
			throw new Exception(msg.ToString());
		}
		return result.CompiledAssembly;
	}
}
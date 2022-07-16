/*
 * Copyright (C) 2006 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Reflection;
using System.Runtime.Serialization;
/*

using System;

using System.Runtime.Serialization;
using System.Reflection;
using System.Collections.Generic;

namespace AndroidUI
{
    public interface ICloneable
    {
        /// <summary>
        /// returns a new UNINITIALIZED instance of this object
        /// <br></br>
        /// <br></br> this is useful when since there is no way to know what constructors the subclasses will have, nor how to call them
        /// </summary>
        public static object Clone(object obj) => CloneImpl(obj);
        public static object Clone<T>() => CloneImpl(FormatterServices.GetUninitializedObject(typeof(T)));
		
		static private object CloneImpl(object obj) {
			if (obj == null) return null;
			
			object o = FormatterServices.GetUninitializedObject(obj.GetType());
			Type t = o.GetType();
			FieldInfo[] fields = t.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fields.Length != 0) {
				Console.WriteLine(t.Name + ": Obtained " + fields.Length + " field");
				foreach (FieldInfo field in fields) {
					Console.WriteLine(t.Name + ": Obtained field: " + field.Name);
					if (!field.IsLiteral) field.SetValue(o, field.GetValue(obj));
					Console.WriteLine(t.Name + ":   value: " + (field.GetValue(obj) ?? "<null>") + ", copy: " + (field.GetValue(o) ?? "<null>"));
				}
			} else {
				Console.WriteLine(t.Name + ": Obtained no fields");
			}

			PropertyInfo[] properties = t.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (properties.Length != 0) {
				Console.WriteLine(t.Name + ": Obtained " + properties.Length + " properties");
				foreach (var prop in properties) {
					Console.WriteLine(t.Name + ": Obtained property: " + prop.Name + ", " + prop.GetType().IsGenericType + ", " + prop.DeclaringType.IsGenericType + ", " + prop.PropertyType.IsGenericType + ", " + prop.ReflectedType.IsGenericType);
					if (prop.CanRead && prop.CanWrite) prop.SetValue(o, prop.GetValue(obj));
					Console.WriteLine(t.Name + ":   value: " + (prop.CanRead ? (prop.GetValue(obj)?.ToString() ?? "<null>") : "<cannot read>") + ", copy: " + (prop.CanRead ? (prop.GetValue(o)?.ToString() ?? "<null>") : "<cannot read>"));
				}
			} else {
				Console.WriteLine(t.Name + ": Obtained no fields");
			}
			
			return o;
		}

        /// <summary>
        /// when overriding this class, `virtual public YOUR_CLASS_HERE Clone()` if you directly implement ICloneable, call ICloneable.Clone(this)
        /// <br></br><br></br><inheritdoc cref="Clone(object)"/>
        /// </summary>
        public virtual object Clone() => CloneImpl(this);
    }
}

public class Program
{
	
	class NoFields {
	}
	
	class Field {
		Action<int> action = new Action<int>((i) => {});
		int a_p => 5;
		ValueType a;
		const int const_field = 1;
		readonly int readonly_field = 2;
		internal int internal_field = 3;
		private int private_field = 4;
		protected int protected_field = 5;
		int field = 6;
		public int public_field = 7;
	}
	
	class Property {
		internal int internal_field { get; set; }
		private int private_field { get; set; }
		protected int protected_field { get; set; }
		int field { get; set; }
		public int public_field { get; set; }
	}
	
	class PropertyBackedByField {
		internal int internal_field;
		private int private_field;
		protected int protected_field;
		int field;
		public int public_field;
		
		internal int internal_field_p { get => internal_field; set => internal_field = value; }
		private int private_field_p { get => private_field; set => private_field = value; }
		protected int protected_field_p { get => protected_field; set => protected_field = value; }
		int field_p { get => field; set => field = value; }
		public int public_field_p { get => public_field; set => public_field = value; }
	}
	
	class Field_Inherited : Field {
		internal int internal_field;
		private int private_field;
		protected int protected_field;
		int field;
		public int public_field;
	}
	
	class Field_Inherited_With_New : Field {
		new internal int internal_field;
		private int private_field;
		new protected int protected_field;
		int field;
		new public int public_field;
	}
	
	void main()
	{
		//AndroidUI.ICloneable.Clone<NoFields>();
		//AndroidUI.ICloneable.Clone(new Field());
		//AndroidUI.ICloneable.Clone<Property>();
		//AndroidUI.ICloneable.Clone<PropertyBackedByField>();
		//AndroidUI.ICloneable.Clone<Field_Inherited>();
		//AndroidUI.ICloneable.Clone<Field_Inherited_With_New>();
		var hash = new object();
		//var clonedHash = AndroidUI.ICloneable.Clone(hash);
		var clonedHash = Force.DeepCloner.DeepClonerExtensions.DeepClone(hash);
		Console.WriteLine(hash.GetHashCode().Equals(clonedHash.GetHashCode()));
		//Force.DeepCloner.DeepClonerExtensions.ShallowClone(typeof(int));
		//AndroidUI.ICloneable.Clone(Force.DeepCloner.DeepClonerExtensions.DeepClone(typeof(int)));
	}

	public static void Main()
	{
		new Program().main();
	}
}

*/
namespace AndroidUI.Utils
{
    public interface ICloneable
    {
        /// <summary>
        /// returns a new UNINITIALIZED instance of this object
        /// <br></br>
        /// <br></br> this is useful when since there is no way to know what constructors the subclasses will have, nor how to call them
        /// </summary>
        public static object Clone(object obj) => obj == null ? null : FormatterServices.GetUninitializedObject(obj.GetType());

        /// <summary>
        /// when overriding this class, `virtual public YOUR_CLASS_HERE Clone()` if you directly implement ICloneable, call ICloneable.Clone(this)
        /// <br></br><br></br><inheritdoc cref="Clone(object)"/>
        /// </summary>
        public virtual object Clone() => Clone(this); // incase someone does not override this
    }
}

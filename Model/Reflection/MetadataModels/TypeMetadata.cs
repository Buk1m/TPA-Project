﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Model.Reflection.Enums;

namespace Model.Reflection.MetadataModels
{
    [DataContract(IsReference = true)]
    public class TypeMetadata
    {
        #region Constructor

        internal TypeMetadata(Type type)
        {
            // Types
            BaseType = EmitExtends(type.GetTypeInfo().BaseType);
            DeclaringType = EmitType(type.DeclaringType);
            NestedTypes = EmitTypes(type.GetNestedTypes(AllAccessLevels));
            Attributes = EmitAttributes(type.GetCustomAttributes(false).Cast<Attribute>());
            Fields = EmitFields(type.GetFields(AllAccessLevels));
            Events = EmitEvents(type.GetEvents(AllAccessLevels));
            GenericArguments = type.IsGenericTypeDefinition ? EmitGenericArguments(type.GetGenericArguments()) : null;
            Properties = EmitProperties(type.GetProperties(AllAccessLevels));
            ImplementedInterfaces = EmitTypes(type.GetInterfaces());

            // Methods
            Constructors = MethodMetadata.EmitMethods(type.GetConstructors(AllAccessLevels));
            Methods = MethodMetadata.EmitMethods(type.GetMethods(AllAccessLevels));

            // Infos
            TypeName = type.Name;
            FullName = type.FullName ?? type.Namespace + "." + type.Name;
            Modifiers = EmitModifiers(type);
            TypeKind = GetTypeKind(type);
        }

        internal static IEnumerable<TypeMetadata> EmitGenericArguments(IEnumerable<Type> arguments)
        {
            return from Type _argument in arguments select EmitReference(_argument);
        }

        #endregion

        #region Emit API

        internal static IEnumerable<TypeMetadata> EmitTypes(IEnumerable<Type> types)
        {
            return from type in types select EmitType(type);
        }

        internal static TypeMetadata EmitType(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (!TypesDictionary.ReflectedTypes.ContainsKey(type.FullName ?? type.Namespace + " . " + type.Name))
            {
                TypesDictionary.ReflectedTypes.Add(type.FullName ?? type.Namespace + " . " + type.Name,
                    new TypeMetadata(type));
            }

            return TypesDictionary.ReflectedTypes[type.FullName ?? type.Namespace + " . " + type.Name];
        }

        internal static IEnumerable<TypeMetadata> EmitAttributes(IEnumerable<Attribute> attributes)
        {
            if (attributes == null) return null;
            var attributesTypes = from attribute in attributes select attribute.GetType();
            return EmitTypes(attributesTypes);
        }

        #endregion

        #region Private Constructors

        private TypeMetadata(string typeName, string namespaceName)
        {
            TypeName = typeName;
            NamespaceName = namespaceName;
        }

        private TypeMetadata(string typeName, string namespaceName,
            IEnumerable<TypeMetadata> genericArguments) :
            this(typeName, namespaceName)
        {
            GenericArguments = genericArguments;
        }

        #endregion

        #region Private Methods

        private static IEnumerable<FieldMetadata> EmitFields(IEnumerable<FieldInfo> fieldsInfo)
        {
            return from fieldInfo in fieldsInfo select new FieldMetadata(fieldInfo);
        }

        private static IEnumerable<EventMetadata> EmitEvents(IEnumerable<EventInfo> events)
        {
            return from singleEvent in events select new EventMetadata(singleEvent);
        }

        private static IEnumerable<PropertyMetadata> EmitProperties(IEnumerable<PropertyInfo> props)
        {
            return from prop in props select new PropertyMetadata(prop);
        }

        private static TypeKind GetTypeKind(Type type)
        {
            return type.IsEnum ? TypeKind.Enum :
                type.IsValueType ? TypeKind.Struct :
                type.IsInterface ? TypeKind.Interface :
                TypeKind.Class;
        }

        private static Tuple<AccessLevel, SealedEnum, AbstractEnum> EmitModifiers(Type type)
        {
            AccessLevel access = AccessLevel.Private;
            if (type.IsPublic)
                access = AccessLevel.Public;
            else if (type.IsNestedPublic)
                access = AccessLevel.Public;
            else if (type.IsNestedFamily)
                access = AccessLevel.Protected;
            else if (type.IsNestedFamANDAssem)
                access = AccessLevel.Internal;

            SealedEnum _sealed = SealedEnum.NotSealed;
            if (type.IsSealed) _sealed = SealedEnum.Sealed;

            AbstractEnum _abstract = AbstractEnum.NotAbstract;
            if (type.IsAbstract)
                _abstract = AbstractEnum.Abstract;

            return new Tuple<AccessLevel, SealedEnum, AbstractEnum>(access, _sealed, _abstract);
        }

        private static TypeMetadata EmitReference(Type type)
        {
            if (!type.IsGenericType)
            {
                return new TypeMetadata(type.Name, type.GetNamespace());
            }

            return new TypeMetadata(type.Name, type.GetNamespace(),
                EmitGenericArguments(type.GetGenericArguments()));
        }

        private static TypeMetadata EmitExtends(Type baseType)
        {
            if (baseType == null || baseType == typeof(Object) || baseType == typeof(ValueType) ||
                baseType == typeof(Enum))
            {
                return null;
            }

            return EmitReference(baseType);
        }

        #endregion

        #region Variables

        [DataMember] public string TypeName { get; internal set; }
        [DataMember] public string NamespaceName { get; internal set; }
        [DataMember] public TypeMetadata BaseType { get; internal set; }
        [DataMember] public TypeMetadata DeclaringType { get; internal set; }
        [DataMember] public TypeKind TypeKind { get; internal set; }
        [DataMember] public Tuple<AccessLevel, SealedEnum, AbstractEnum> Modifiers { get; internal set; }
        [DataMember] public IEnumerable<FieldMetadata> Fields { get; internal set; }
        [DataMember] public IEnumerable<TypeMetadata> GenericArguments { get; internal set; }
        [DataMember] public IEnumerable<TypeMetadata> Attributes { get; internal set; }
        [DataMember] public IEnumerable<TypeMetadata> ImplementedInterfaces { get; internal set; }
        [DataMember] public IEnumerable<TypeMetadata> NestedTypes { get; internal set; }
        [DataMember] public IEnumerable<PropertyMetadata> Properties { get; internal set; }
        [DataMember] public IEnumerable<MethodMetadata> Methods { get; internal set; }
        [DataMember] public IEnumerable<MethodMetadata> Constructors { get; internal set; }
        [DataMember] public IEnumerable<EventMetadata> Events { get; internal set; }
        [DataMember] public string FullName { get; internal set; }

        const BindingFlags AllAccessLevels = BindingFlags.NonPublic
                                             | BindingFlags.DeclaredOnly
                                             | BindingFlags.Public
                                             | BindingFlags.Static
                                             | BindingFlags.Instance;

        #endregion
    }
}
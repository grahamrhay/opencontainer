#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */

#endregion

using System;
using NUnit.Framework;
using OpenContainer.ContextStore;
using OpenContainer.Extensions;

namespace OpenContainer.Tests.Unit
{
    public abstract class when_resolving_instances : dependency_resolver_context
    {
        public class TypeWithDependencyResolverAsProperty
        {
            public IContainer Resolver { get; set; }
        }
        public class TypeWithPropertyAlreadySet
        {
            public TypeWithPropertyAlreadySet()
            {
				Resolver = new DefaultContainer();
            }
            public IContainer Resolver { get; set; }
        }
        [Test]
        public void a_property_that_would_cause_a_cyclic_dependency_is_ignored()
        {
            Resolver.AddDependency<RecursiveProperty>();

            Resolver.Resolve<RecursiveProperty>().Property.
                ShouldBeNull();
        }

        [Test]
        public void a_type_cannot_be_created_when_its_dependencies_are_not_registered()
        {
            Resolver.AddDependency<IAnother, Another>();

            Executing(() => Resolver.Resolve<IAnother>())
                .ShouldThrow<DependencyResolutionException>();
        }

        [Test]
        public void an_empty_enumeration_of_unregistered_types_is_resolved()
        {
            var simpleList = Resolver.ResolveAll<ISimple>();
            
            simpleList.ShouldNotBeNull();
            simpleList.ShouldBeEmpty();
        }

        [Test]
        public void a_type_can_get_a_dependency_resolver_dependency_assigned()
        {
            Resolver.AddDependencyInstance(typeof (IContainer), Resolver);
            Resolver.AddDependency<TypeWithDependencyResolverAsProperty>(DependencyLifetime.Transient);

            Resolver.Resolve<TypeWithDependencyResolverAsProperty>()
                .Resolver.ShouldBeTheSameInstanceAs(Resolver);
        }
        [Test]
        public void a_property_for_which_there_is_a_property_already_assigned_is_replaced_with_value_from_container()
        {
            Resolver.AddDependencyInstance(typeof(IContainer),Resolver);
            Resolver.AddDependency<TypeWithPropertyAlreadySet>(DependencyLifetime.Singleton);

            Resolver.Resolve<TypeWithPropertyAlreadySet>()
                .Resolver.ShouldBeTheSameInstanceAs(Resolver);
        }
    }

    public abstract class when_registering_dependencies : dependency_resolver_context
    {
        [Test]
        public void an_abstract_type_cannot_be_registered()
        {
            Executing(() => Resolver.AddDependency<ISimple, SimpleAbstract>())
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void an_interface_cannot_be_registered_as_a_concrete_implementation()
        {
            Executing(() => Resolver.AddDependency<ISimple, IAnotherSimple>())
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void cyclic_dependency_generates_an_error()
        {
            Resolver.AddDependency<RecursiveConstructor>();

            Executing(() => Resolver.Resolve<RecursiveConstructor>())
                .ShouldThrow<DependencyResolutionException>();
        }

        [Test]
        public void parameters_are_resolved()
        {
            Resolver.AddDependency<ISimple, Simple>();
            Resolver.AddDependency<ISimpleChild, SimpleChild>();

            ((Simple) Resolver.Resolve<ISimple>()).Property
                .ShouldNotBeNull()
                .ShouldBeOfType<SimpleChild>();
        }

        [Test]
        public void registered_concrete_type_is_recognized_as_dependency_implementation()
        {
            Resolver.AddDependency<ISimple, Simple>();

            Resolver.HasDependencyImplementation(typeof (ISimple), typeof (Simple)).ShouldBeTrue();
        }

        [Test]
        public void registering_a_concrete_type_results_in_the_type_being_registered()
        {
            Resolver.AddDependency(typeof (Simple), DependencyLifetime.Transient);
            Resolver.HasDependency(typeof (Simple))
                .ShouldBeTrue();
        }

        [Test]
        public void registering_a_concrete_type_with_an_unknown_dependency_lifetime_value_results_in__an_error()
        {
            Executing(() => Resolver.AddDependency<Simple>((DependencyLifetime) 999))
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void registering_a_service_type_with_an_unknown_dependency_lifetime_value_results_in__an_error()
        {
            Executing(() => Resolver.AddDependency<ISimple, Simple>((DependencyLifetime) 999))
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void requesting_a_type_with_a_public_constructor_returns_a_new_instance_of_that_type()
        {
            Resolver.AddDependency<ISimple, Simple>();
            Resolver.Resolve<ISimple>().ShouldBeOfType<Simple>();
        }

        [Test]
        public void requesting_a_type_with_no_public_constructor_will_return_a_type_with_the_correct_dependency()
        {
            Resolver.AddDependency<ISimple, Simple>();
            Resolver.AddDependency<IAnother, Another>();

            ((Another) Resolver.Resolve<IAnother>()).Dependent.ShouldBeOfType<Simple>();
        }

        [Test]
        public void the_constructor_with_the_most_matching_arguments_is_used()
        {
            Resolver.AddDependency<ISimple, Simple>();
            Resolver.AddDependency<IAnother, Another>();
            Resolver.AddDependency<TypeWithTwoConstructors>();

            var type = Resolver.Resolve<TypeWithTwoConstructors>();
            type._argOne.ShouldNotBeNull();
            type._argTwo.ShouldNotBeNull();
        }

        [Test]
        public void the_null_value_is_never_registered()
        {
            Resolver.HasDependency(null).ShouldBeFalse();
        }

        [Test]
        public void the_resolved_instance_is_the_same_as_the_registered_instance()
        {
            string objectInstance = "some object";

            Resolver.AddDependencyInstance(typeof (string), objectInstance);

            Resolver.Resolve<string>().ShouldBe(objectInstance);
        }
    }
}

#region Full license

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion
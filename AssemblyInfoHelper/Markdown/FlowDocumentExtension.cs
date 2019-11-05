using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.Windows.Documents;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AssemblyInfoHelper.Markdown
{
    public static class FlowDocumentExtension
    {
        //public static void EnableAllHyperlinks(this FlowDocument document)
        //{
        //    //FieldInfo field = typeof(System.Windows.ContentElement).GetField("_isEnabledCore", BindingFlags.NonPublic | BindingFlags.Instance);
        //    //PropertyInfo property = typeof(System.Windows.ContentElement).GetProperty("IsEnabledCore");

        //    //see: https://www.productiverage.com/trying-to-set-a-readonly-autoproperty-value-externally-plus-a-little-benchmarkdotnet
        //    var type = typeof(System.Windows.ContentElement);
        //    var property = type.GetProperty("IsEnabled");

        //    var backingField = type
        //      .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        //      .FirstOrDefault(field =>
        //        field.Attributes.HasFlag(FieldAttributes.Private) &&
        //        field.Attributes.HasFlag(FieldAttributes.InitOnly) &&
        //        field.CustomAttributes.Any(attr => attr.AttributeType == typeof(CompilerGeneratedAttribute)) &&
        //        (field.DeclaringType == property.DeclaringType) &&
        //        field.FieldType.IsAssignableFrom(property.PropertyType) &&
        //        field.Name.StartsWith("<" + property.Name + ">")
        //      );

        //    EnableHyperlink(document, property, FlowDocumentVisitors);
        //}

        /// <summary>
        /// Attach the FlowDocument to the current thread. The dispatcher of all FlowDocument elements are changed to the dispatcher of the current thread.
        /// This needs to be done to avoid the "The calling thread cannot access this object because a different thread owns it" error.
        /// </summary>
        /// <param name="document">The FlowDocument to attach</param>
        /// see: https://turecki.net/flowdocument-from-a-different-thread
        public static void AttachToCurrentThread(this FlowDocument document)
        {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            FieldInfo field = typeof(DispatcherObject).GetField("_dispatcher", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("_dispatcher field missing on DispatcherObject");
            }
            SetDispatcher(document, field, dispatcher, FlowDocumentVisitors);

            PropertyInfo property = typeof(ContentElement).GetProperty("IsEnabled");
            if (property == null)
            {
                throw new InvalidOperationException("IsEnabled property missing on ContentElement");
            }
            EnableAllElements(document, property, FlowDocumentVisitors);
        }

        private static readonly Func<object, object>[] FlowDocumentVisitors =
        {
            x => (x is FlowDocument) ? ((FlowDocument) x).Blocks : null,
            x => (x is Section) ? ((Section) x).Blocks : null,
            x => (x is BlockUIContainer) ? ((BlockUIContainer) x).Child : null,
            x => (x is InlineUIContainer) ? ((InlineUIContainer) x).Child : null,
            x => (x is Span) ? ((Span) x).Inlines : null,
            x => (x is AnchoredBlock) ? ((AnchoredBlock) x).Blocks : null,
            x => (x is Paragraph) ? ((Paragraph) x).Inlines : null,
            x => (x is Table) ? ((Table) x).RowGroups : null,
            x => (x is Table) ? ((Table) x).Columns : null,
            x => (x is Table) ? ((Table) x).RowGroups.SelectMany(rg => rg.Rows) : null,
            x => (x is Table) ? ((Table) x).RowGroups.SelectMany(rg => rg.Rows).SelectMany(r => r.Cells) : null,
            x => (x is TableCell) ? ((TableCell) x).Blocks : null,
            x => (x is TableCell) ? ((TableCell) x).BorderBrush : null,
            x => (x is List) ? ((List) x).ListItems : null,
            x => (x is ListItem) ? ((ListItem) x).Blocks : null
        };

        private static void SetDispatcher(object item, FieldInfo field, object value, params Func<object, object>[] selectors)
        {
            if (item is DispatcherObject)
            {
                Dispatcher currentDispatcher = field.GetValue(item) as Dispatcher;
                if (currentDispatcher != null && currentDispatcher != value)
                {
                    field.SetValue(item, value);
                }
            }
            if (item is IEnumerable)
            {
                foreach (var subItem in item as IEnumerable)
                {
                    SetDispatcher(subItem, field, value, selectors);
                }
            }
            if (selectors != null)
            {
                foreach (var selector in selectors.Select(x => x(item)).Where(x => x != null))
                {
                    SetDispatcher(selector, field, value, selectors);
                }
            }
        }

        private static void EnableAllElements(object item, PropertyInfo property, params Func<object, object>[] selectors)
        {
            if (item is ContentElement)
            {
                property.SetValue(item, true);
            }
            if (item is IEnumerable)
            {
                foreach (var subItem in item as IEnumerable)
                {
                    EnableAllElements(subItem, property, selectors);
                }
            }
            if (selectors != null)
            {
                foreach (var selector in selectors.Select(x => x(item)).Where(x => x != null))
                {
                    EnableAllElements(selector, property, selectors);
                }
            }
        }
    }
}

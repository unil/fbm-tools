﻿#pragma checksum "..\..\..\AjouterTraductionWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8EBB26B4D6431F8A4D1F4AD98F61CAB0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Folder_Rename_Wpf {
    
    
    /// <summary>
    /// AjouterTraductionWindow
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class AjouterTraductionWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\AjouterTraductionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox groupBoxExistantes;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\AjouterTraductionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxTraductions;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\AjouterTraductionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox groupBoxNouvelle;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\AjouterTraductionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBoxValeur;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\AjouterTraductionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBoxValeurTraduite;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\AjouterTraductionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAjouter;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\AjouterTraductionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label1;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\AjouterTraductionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnFermer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Folder_Rename_Wpf;component/ajoutertraductionwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\AjouterTraductionWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 7 "..\..\..\AjouterTraductionWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.refresh_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.groupBoxExistantes = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 3:
            this.listBoxTraductions = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.groupBoxNouvelle = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 5:
            this.txtBoxValeur = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.txtBoxValeurTraduite = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.btnAjouter = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\..\AjouterTraductionWindow.xaml"
            this.btnAjouter.Click += new System.Windows.RoutedEventHandler(this.btnAjouter_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.label1 = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.btnFermer = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\..\AjouterTraductionWindow.xaml"
            this.btnFermer.Click += new System.Windows.RoutedEventHandler(this.btnFermer_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


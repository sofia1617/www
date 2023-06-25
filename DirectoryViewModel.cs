using CV19.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;

namespace CV19.ViewModels
{
    internal class DirectoryViewModel : ViewModel
    {
        private readonly DirectoryInfo _DirectoryInfo;

        public IEnumerable<DirectoryViewModel> SubDirectories {
            get {
                try
                {
                    var directories = _DirectoryInfo.EnumerateDirectories().Select( dir_info => new DirectoryViewModel( dir_info.FullName ) );
                    return directories;
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine( e.ToString() );
                }
                return Enumerable.Empty<DirectoryViewModel>();
            }
        }

        public IEnumerable<FileViewModel> Files {
            get {
                try
                {
                    var files = _DirectoryInfo.EnumerateFiles().Select( dir_info => new FileViewModel( dir_info.FullName ) );
                    return files;
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine( e.ToString() );
                }
                return Enumerable.Empty<FileViewModel>();
            }
        }

        public IEnumerable<object> DirectoryItems {
            get {
                try
                {
                    var directories = SubDirectories.Cast<object>().Concat( Files.Cast<object>() );
                    return directories;
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine( e.ToString() );
                }
                return Enumerable.Empty<object>();
            }
        }

        public string Name => _DirectoryInfo.Name;
        public string Path => _DirectoryInfo.FullName;
        public DateTime CreationTIme => _DirectoryInfo.CreationTime;



        public DirectoryViewModel( string Path )
        {
            _DirectoryInfo = new DirectoryInfo( Path );
        }
    }

    internal class FileViewModel : ViewModel
    {
        private readonly FileInfo _FileInfo;

        public string Name => _FileInfo.Name;
        public string Path => _FileInfo.FullName;
        public DateTime CreationTIme => _FileInfo.CreationTime;



        public FileViewModel( string Path )
        {
            _FileInfo = new FileInfo( Path );
        }
    }
}

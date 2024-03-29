﻿using System;
using System.Collections.Generic;
using YourVitebskWebServiceApp.ViewModels;

namespace YourVitebskWebServiceApp.Interfaces
{
    public interface ICommentRepository : IPermissionChecker, IDisposable
    {
        IEnumerable<CommentViewModel> Get();
        CommentViewModel Get(int id);
        void Delete(int id);
    }
}

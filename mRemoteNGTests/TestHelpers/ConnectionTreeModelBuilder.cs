﻿using mRemoteNG.App;
using mRemoteNG.Connection;
using mRemoteNG.Container;
using mRemoteNG.Credential;
using mRemoteNG.Security;
using mRemoteNG.Tools;
using mRemoteNG.Tree;
using mRemoteNG.Tree.Root;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace mRemoteNGTests.TestHelpers
{
    public class ConnectionTreeModelBuilder
    {
        /// <summary>
        /// Builds a tree which looks like:
        /// Root
        /// |- folder1
        /// |   |- con1
        /// |- con2
        /// |- folder2
        ///     |- folder3
        ///         |- con3
        /// </summary>
        /// <returns></returns>
        public ConnectionTreeModel Build()
        {
            var cred1 = new CredentialRecord {Username = "user1", Domain = "domain1", Password = "password1".ConvertToSecureString()};
            var cred2 = new CredentialRecord {Username = "user2", Domain = "domain2", Password = "password2".ConvertToSecureString()};

            var credRepo = Substitute.For<ICredentialRepository>();
            credRepo.CredentialRecords.Returns(info => new List<ICredentialRecord> {cred1, cred2});

            Runtime.CredentialService.RepositoryList.ToArray().ForEach(r => Runtime.CredentialService.RemoveRepository(r));
            Runtime.CredentialService.AddRepository(credRepo);


            var model = new ConnectionTreeModel();
            var root = new RootNodeInfo(RootNodeType.Connection);
            var folder1 = new ContainerInfo { Name = "folder1", CredentialRecord = cred1 };
            var folder2 = new ContainerInfo { Name = "folder2", CredentialRecord = cred2 };
            var folder3 = new ContainerInfo
            {
                Name = "folder3",
                Inheritance =
                {
                    CredentialId = true
                }
            };
            var con1 = new ConnectionInfo { Name = "Con1", CredentialRecord = cred1 };
            var con2 = new ConnectionInfo { Name = "Con2", CredentialRecord = cred2 };
            var con3 = new ContainerInfo
            {
                Name = "con3",
                Inheritance =
                {
                    CredentialId = true
                }
            };

            root.AddChild(folder1);
            root.AddChild(con2);
            folder1.AddChild(con1);
            root.AddChild(folder2);
            folder2.AddChild(folder3);
            folder3.AddChild(con3);
            model.AddRootNode(root);
            return model;
        }
    }
}
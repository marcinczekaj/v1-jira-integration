/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;

namespace VersionOne.Jira.SoapProxy.Jira
{
	public interface IJiraSoapService : IDisposable
	{
		string Url { get; set; }

		bool UseDefaultCredentials { get; set; }

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getCommentReturn")]
		RemoteComment getComment(string in0, long in1);

		/// <remarks/>
		void getCommentAsync(string in0, long in1);

		/// <remarks/>
		void getCommentAsync(string in0, long in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("createGroupReturn")]
		RemoteGroup createGroup(string in0, string in1, RemoteUser in2);

		/// <remarks/>
		void createGroupAsync(string in0, string in1, RemoteUser in2);

		/// <remarks/>
		void createGroupAsync(string in0, string in1, RemoteUser in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getServerInfoReturn")]
		RemoteServerInfo getServerInfo(string in0);

		/// <remarks/>
		void getServerInfoAsync(string in0);

		/// <remarks/>
		void getServerInfoAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getGroupReturn")]
		RemoteGroup getGroup(string in0, string in1);

		/// <remarks/>
		void getGroupAsync(string in0, string in1);

		/// <remarks/>
		void getGroupAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("loginReturn")]
		string login(string in0, string in1);

		/// <remarks/>
		void loginAsync(string in0, string in1);

		/// <remarks/>
		void loginAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getUserReturn")]
		RemoteUser getUser(string in0, string in1);

		/// <remarks/>
		void getUserAsync(string in0, string in1);

		/// <remarks/>
		void getUserAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("createUserReturn")]
		RemoteUser createUser(string in0, string in1, string in2, string in3, string in4);

		/// <remarks/>
		void createUserAsync(string in0, string in1, string in2, string in3, string in4);

		/// <remarks/>
		void createUserAsync(string in0, string in1, string in2, string in3, string in4, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getIssueReturn")]
		RemoteIssue getIssue(string in0, string in1);

		/// <remarks/>
		void getIssueAsync(string in0, string in1);

		/// <remarks/>
		void getIssueAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("createIssueReturn")]
		RemoteIssue createIssue(string in0, RemoteIssue in1);

		/// <remarks/>
		void createIssueAsync(string in0, RemoteIssue in1);

		/// <remarks/>
		void createIssueAsync(string in0, RemoteIssue in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getAvailableActionsReturn")]
		RemoteNamedObject[] getAvailableActions(string in0, string in1);

		/// <remarks/>
		void getAvailableActionsAsync(string in0, string in1);

		/// <remarks/>
		void getAvailableActionsAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getProjectsReturn")]
		RemoteProject[] getProjects(string in0);

		/// <remarks/>
		void getProjectsAsync(string in0);

		/// <remarks/>
		void getProjectsAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("updateIssueReturn")]
		RemoteIssue updateIssue(string in0, string in1, RemoteFieldValue[] in2);

		/// <remarks/>
		void updateIssueAsync(string in0, string in1, RemoteFieldValue[] in2);

		/// <remarks/>
		void updateIssueAsync(string in0, string in1, RemoteFieldValue[] in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getConfigurationReturn")]
		RemoteConfiguration getConfiguration(string in0);

		/// <remarks/>
		void getConfigurationAsync(string in0);

		/// <remarks/>
		void getConfigurationAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getComponentsReturn")]
		RemoteComponent[] getComponents(string in0, string in1);

		/// <remarks/>
		void getComponentsAsync(string in0, string in1);

		/// <remarks/>
		void getComponentsAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("updateProjectReturn")]
		RemoteProject updateProject(string in0, RemoteProject in1);

		/// <remarks/>
		void updateProjectAsync(string in0, RemoteProject in1);

		/// <remarks/>
		void updateProjectAsync(string in0, RemoteProject in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getProjectByKeyReturn")]
		RemoteProject getProjectByKey(string in0, string in1);

		/// <remarks/>
		void getProjectByKeyAsync(string in0, string in1);

		/// <remarks/>
		void getProjectByKeyAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getPrioritiesReturn")]
		RemotePriority[] getPriorities(string in0);

		/// <remarks/>
		void getPrioritiesAsync(string in0);

		/// <remarks/>
		void getPrioritiesAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getResolutionsReturn")]
		RemoteResolution[] getResolutions(string in0);

		/// <remarks/>
		void getResolutionsAsync(string in0);

		/// <remarks/>
		void getResolutionsAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getIssueTypesReturn")]
		RemoteIssueType[] getIssueTypes(string in0);

		/// <remarks/>
		void getIssueTypesAsync(string in0);

		/// <remarks/>
		void getIssueTypesAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getStatusesReturn")]
		RemoteStatus[] getStatuses(string in0);

		/// <remarks/>
		void getStatusesAsync(string in0);

		/// <remarks/>
		void getStatusesAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getSubTaskIssueTypesReturn")]
		RemoteIssueType[] getSubTaskIssueTypes(string in0);

		/// <remarks/>
		void getSubTaskIssueTypesAsync(string in0);

		/// <remarks/>
		void getSubTaskIssueTypesAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getProjectRolesReturn")]
		RemoteProjectRole[] getProjectRoles(string in0);

		/// <remarks/>
		void getProjectRolesAsync(string in0);

		/// <remarks/>
		void getProjectRolesAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getProjectRoleReturn")]
		RemoteProjectRole getProjectRole(string in0, long in1);

		/// <remarks/>
		void getProjectRoleAsync(string in0, long in1);

		/// <remarks/>
		void getProjectRoleAsync(string in0, long in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getProjectRoleActorsReturn")]
		RemoteProjectRoleActors getProjectRoleActors(string in0, RemoteProjectRole in1, RemoteProject in2);

		/// <remarks/>
		void getProjectRoleActorsAsync(string in0, RemoteProjectRole in1, RemoteProject in2);

		/// <remarks/>
		void getProjectRoleActorsAsync(string in0, RemoteProjectRole in1, RemoteProject in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getDefaultRoleActorsReturn")]
		RemoteRoleActors getDefaultRoleActors(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void getDefaultRoleActorsAsync(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void getDefaultRoleActorsAsync(string in0, RemoteProjectRole in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void removeAllRoleActorsByNameAndType(string in0, string in1, string in2);

		/// <remarks/>
		void removeAllRoleActorsByNameAndTypeAsync(string in0, string in1, string in2);

		/// <remarks/>
		void removeAllRoleActorsByNameAndTypeAsync(string in0, string in1, string in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void removeAllRoleActorsByProject(string in0, RemoteProject in1);

		/// <remarks/>
		void removeAllRoleActorsByProjectAsync(string in0, RemoteProject in1);

		/// <remarks/>
		void removeAllRoleActorsByProjectAsync(string in0, RemoteProject in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void deleteProjectRole(string in0, RemoteProjectRole in1, bool in2);

		/// <remarks/>
		void deleteProjectRoleAsync(string in0, RemoteProjectRole in1, bool in2);

		/// <remarks/>
		void deleteProjectRoleAsync(string in0, RemoteProjectRole in1, bool in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void updateProjectRole(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void updateProjectRoleAsync(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void updateProjectRoleAsync(string in0, RemoteProjectRole in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("createProjectRoleReturn")]
		RemoteProjectRole createProjectRole(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void createProjectRoleAsync(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void createProjectRoleAsync(string in0, RemoteProjectRole in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("isProjectRoleNameUniqueReturn")]
		bool isProjectRoleNameUnique(string in0, string in1);

		/// <remarks/>
		void isProjectRoleNameUniqueAsync(string in0, string in1);

		/// <remarks/>
		void isProjectRoleNameUniqueAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void addActorsToProjectRole(string in0, string[] in1, RemoteProjectRole in2, RemoteProject in3, string in4);

		/// <remarks/>
		void addActorsToProjectRoleAsync(string in0, string[] in1, RemoteProjectRole in2, RemoteProject in3, string in4);

		/// <remarks/>
		void addActorsToProjectRoleAsync(string in0, string[] in1, RemoteProjectRole in2, RemoteProject in3, string in4,
		                                 object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void removeActorsFromProjectRole(string in0, string[] in1, RemoteProjectRole in2, RemoteProject in3, string in4);

		/// <remarks/>
		void removeActorsFromProjectRoleAsync(string in0, string[] in1, RemoteProjectRole in2, RemoteProject in3, string in4);

		/// <remarks/>
		void removeActorsFromProjectRoleAsync(string in0, string[] in1, RemoteProjectRole in2, RemoteProject in3, string in4,
		                                      object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void addDefaultActorsToProjectRole(string in0, string[] in1, RemoteProjectRole in2, string in3);

		/// <remarks/>
		void addDefaultActorsToProjectRoleAsync(string in0, string[] in1, RemoteProjectRole in2, string in3);

		/// <remarks/>
		void addDefaultActorsToProjectRoleAsync(string in0, string[] in1, RemoteProjectRole in2, string in3, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void removeDefaultActorsFromProjectRole(string in0, string[] in1, RemoteProjectRole in2, string in3);

		/// <remarks/>
		void removeDefaultActorsFromProjectRoleAsync(string in0, string[] in1, RemoteProjectRole in2, string in3);

		/// <remarks/>
		void removeDefaultActorsFromProjectRoleAsync(string in0, string[] in1, RemoteProjectRole in2, string in3,
		                                             object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getAssociatedNotificationSchemesReturn")]
		RemoteScheme[] getAssociatedNotificationSchemes(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void getAssociatedNotificationSchemesAsync(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void getAssociatedNotificationSchemesAsync(string in0, RemoteProjectRole in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getAssociatedPermissionSchemesReturn")]
		RemoteScheme[] getAssociatedPermissionSchemes(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void getAssociatedPermissionSchemesAsync(string in0, RemoteProjectRole in1);

		/// <remarks/>
		void getAssociatedPermissionSchemesAsync(string in0, RemoteProjectRole in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getCustomFieldsReturn")]
		RemoteField[] getCustomFields(string in0);

		/// <remarks/>
		void getCustomFieldsAsync(string in0);

		/// <remarks/>
		void getCustomFieldsAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getSavedFiltersReturn")]
		RemoteFilter[] getSavedFilters(string in0);

		/// <remarks/>
		void getSavedFiltersAsync(string in0);

		/// <remarks/>
		void getSavedFiltersAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getCommentsReturn")]
		RemoteComment[] getComments(string in0, string in1);

		/// <remarks/>
		void getCommentsAsync(string in0, string in1);

		/// <remarks/>
		void getCommentsAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void archiveVersion(string in0, string in1, string in2, bool in3);

		/// <remarks/>
		void archiveVersionAsync(string in0, string in1, string in2, bool in3);

		/// <remarks/>
		void archiveVersionAsync(string in0, string in1, string in2, bool in3, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getVersionsReturn")]
		RemoteVersion[] getVersions(string in0, string in1);

		/// <remarks/>
		void getVersionsAsync(string in0, string in1);

		/// <remarks/>
		void getVersionsAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("createProjectReturn")]
		RemoteProject createProject(string in0, string in1, string in2, string in3, string in4, string in5,
		                            RemotePermissionScheme in6, RemoteScheme in7, RemoteScheme in8);

		/// <remarks/>
		void createProjectAsync(string in0, string in1, string in2, string in3, string in4, string in5,
		                        RemotePermissionScheme in6, RemoteScheme in7, RemoteScheme in8);

		/// <remarks/>
		void createProjectAsync(string in0, string in1, string in2, string in3, string in4, string in5,
		                        RemotePermissionScheme in6, RemoteScheme in7, RemoteScheme in8, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void addComment(string in0, string in1, RemoteComment in2);

		/// <remarks/>
		void addCommentAsync(string in0, string in1, RemoteComment in2);

		/// <remarks/>
		void addCommentAsync(string in0, string in1, RemoteComment in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getFieldsForEditReturn")]
		RemoteField[] getFieldsForEdit(string in0, string in1);

		/// <remarks/>
		void getFieldsForEditAsync(string in0, string in1);

		/// <remarks/>
		void getFieldsForEditAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void addUserToGroup(string in0, RemoteGroup in1, RemoteUser in2);

		/// <remarks/>
		void addUserToGroupAsync(string in0, RemoteGroup in1, RemoteUser in2);

		/// <remarks/>
		void addUserToGroupAsync(string in0, RemoteGroup in1, RemoteUser in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void removeUserFromGroup(string in0, RemoteGroup in1, RemoteUser in2);

		/// <remarks/>
		void removeUserFromGroupAsync(string in0, RemoteGroup in1, RemoteUser in2);

		/// <remarks/>
		void removeUserFromGroupAsync(string in0, RemoteGroup in1, RemoteUser in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("logoutReturn")]
		bool logout(string in0);

		/// <remarks/>
		void logoutAsync(string in0);

		/// <remarks/>
		void logoutAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getProjectByIdReturn")]
		RemoteProject getProjectById(string in0, long in1);

		/// <remarks/>
		void getProjectByIdAsync(string in0, long in1);

		/// <remarks/>
		void getProjectByIdAsync(string in0, long in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void deleteProject(string in0, string in1);

		/// <remarks/>
		void deleteProjectAsync(string in0, string in1);

		/// <remarks/>
		void deleteProjectAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void releaseVersion(string in0, string in1, RemoteVersion in2);

		/// <remarks/>
		void releaseVersionAsync(string in0, string in1, RemoteVersion in2);

		/// <remarks/>
		void releaseVersionAsync(string in0, string in1, RemoteVersion in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void deleteIssue(string in0, string in1);

		/// <remarks/>
		void deleteIssueAsync(string in0, string in1);

		/// <remarks/>
		void deleteIssueAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("addAttachmentsToIssueReturn")]
		bool addAttachmentsToIssue(string in0, string in1, string[] in2, sbyte[] in3);

		/// <remarks/>
		void addAttachmentsToIssueAsync(string in0, string in1, string[] in2, sbyte[] in3);

		/// <remarks/>
		void addAttachmentsToIssueAsync(string in0, string in1, string[] in2, sbyte[] in3, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getAttachmentsFromIssueReturn")]
		RemoteAttachment[] getAttachmentsFromIssue(string in0, string in1);

		/// <remarks/>
		void getAttachmentsFromIssueAsync(string in0, string in1);

		/// <remarks/>
		void getAttachmentsFromIssueAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("hasPermissionToEditCommentReturn")]
		bool hasPermissionToEditComment(string in0, RemoteComment in1);

		/// <remarks/>
		void hasPermissionToEditCommentAsync(string in0, RemoteComment in1);

		/// <remarks/>
		void hasPermissionToEditCommentAsync(string in0, RemoteComment in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("editCommentReturn")]
		RemoteComment editComment(string in0, RemoteComment in1);

		/// <remarks/>
		void editCommentAsync(string in0, RemoteComment in1);

		/// <remarks/>
		void editCommentAsync(string in0, RemoteComment in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getFieldsForActionReturn")]
		RemoteField[] getFieldsForAction(string in0, string in1, string in2);

		/// <remarks/>
		void getFieldsForActionAsync(string in0, string in1, string in2);

		/// <remarks/>
		void getFieldsForActionAsync(string in0, string in1, string in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("progressWorkflowActionReturn")]
		RemoteIssue progressWorkflowAction(string in0, string in1, string in2, RemoteFieldValue[] in3);

		/// <remarks/>
		void progressWorkflowActionAsync(string in0, string in1, string in2, RemoteFieldValue[] in3);

		/// <remarks/>
		void progressWorkflowActionAsync(string in0, string in1, string in2, RemoteFieldValue[] in3, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getIssueByIdReturn")]
		RemoteIssue getIssueById(string in0, string in1);

		/// <remarks/>
		void getIssueByIdAsync(string in0, string in1);

		/// <remarks/>
		void getIssueByIdAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void addWorklogWithNewRemainingEstimate(string in0, string in1, RemoteWorklog in2, string in3);

		/// <remarks/>
		void addWorklogWithNewRemainingEstimateAsync(string in0, string in1, RemoteWorklog in2, string in3);

		/// <remarks/>
		void addWorklogWithNewRemainingEstimateAsync(string in0, string in1, RemoteWorklog in2, string in3, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void addWorklogAndAutoAdjustRemainingEstimate(string in0, string in1, RemoteWorklog in2);

		/// <remarks/>
		void addWorklogAndAutoAdjustRemainingEstimateAsync(string in0, string in1, RemoteWorklog in2);

		/// <remarks/>
		void addWorklogAndAutoAdjustRemainingEstimateAsync(string in0, string in1, RemoteWorklog in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void addWorklogAndRetainRemainingEstimate(string in0, string in1, RemoteWorklog in2);

		/// <remarks/>
		void addWorklogAndRetainRemainingEstimateAsync(string in0, string in1, RemoteWorklog in2);

		/// <remarks/>
		void addWorklogAndRetainRemainingEstimateAsync(string in0, string in1, RemoteWorklog in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void deleteWorklogWithNewRemainingEstimate(string in0, string in1, string in2);

		/// <remarks/>
		void deleteWorklogWithNewRemainingEstimateAsync(string in0, string in1, string in2);

		/// <remarks/>
		void deleteWorklogWithNewRemainingEstimateAsync(string in0, string in1, string in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void deleteWorklogAndAutoAdjustRemainingEstimate(string in0, string in1);

		/// <remarks/>
		void deleteWorklogAndAutoAdjustRemainingEstimateAsync(string in0, string in1);

		/// <remarks/>
		void deleteWorklogAndAutoAdjustRemainingEstimateAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void deleteWorklogAndRetainRemainingEstimate(string in0, string in1);

		/// <remarks/>
		void deleteWorklogAndRetainRemainingEstimateAsync(string in0, string in1);

		/// <remarks/>
		void deleteWorklogAndRetainRemainingEstimateAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void updateWorklogWithNewRemainingEstimate(string in0, RemoteWorklog in1, string in2);

		/// <remarks/>
		void updateWorklogWithNewRemainingEstimateAsync(string in0, RemoteWorklog in1, string in2);

		/// <remarks/>
		void updateWorklogWithNewRemainingEstimateAsync(string in0, RemoteWorklog in1, string in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void updateWorklogAndAutoAdjustRemainingEstimate(string in0, RemoteWorklog in1);

		/// <remarks/>
		void updateWorklogAndAutoAdjustRemainingEstimateAsync(string in0, RemoteWorklog in1);

		/// <remarks/>
		void updateWorklogAndAutoAdjustRemainingEstimateAsync(string in0, RemoteWorklog in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void updateWorklogAndRetainRemainingEstimate(string in0, RemoteWorklog in1);

		/// <remarks/>
		void updateWorklogAndRetainRemainingEstimateAsync(string in0, RemoteWorklog in1);

		/// <remarks/>
		void updateWorklogAndRetainRemainingEstimateAsync(string in0, RemoteWorklog in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getWorklogsReturn")]
		RemoteWorklog[] getWorklogs(string in0, string in1);

		/// <remarks/>
		void getWorklogsAsync(string in0, string in1);

		/// <remarks/>
		void getWorklogsAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("hasPermissionToCreateWorklogReturn")]
		bool hasPermissionToCreateWorklog(string in0, string in1);

		/// <remarks/>
		void hasPermissionToCreateWorklogAsync(string in0, string in1);

		/// <remarks/>
		void hasPermissionToCreateWorklogAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("hasPermissionToDeleteWorklogReturn")]
		bool hasPermissionToDeleteWorklog(string in0, string in1);

		/// <remarks/>
		void hasPermissionToDeleteWorklogAsync(string in0, string in1);

		/// <remarks/>
		void hasPermissionToDeleteWorklogAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("hasPermissionToUpdateWorklogReturn")]
		bool hasPermissionToUpdateWorklog(string in0, string in1);

		/// <remarks/>
		void hasPermissionToUpdateWorklogAsync(string in0, string in1);

		/// <remarks/>
		void hasPermissionToUpdateWorklogAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getNotificationSchemesReturn")]
		RemoteScheme[] getNotificationSchemes(string in0);

		/// <remarks/>
		void getNotificationSchemesAsync(string in0);

		/// <remarks/>
		void getNotificationSchemesAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getPermissionSchemesReturn")]
		RemotePermissionScheme[] getPermissionSchemes(string in0);

		/// <remarks/>
		void getPermissionSchemesAsync(string in0);

		/// <remarks/>
		void getPermissionSchemesAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("createPermissionSchemeReturn")]
		RemotePermissionScheme createPermissionScheme(string in0, string in1, string in2);

		/// <remarks/>
		void createPermissionSchemeAsync(string in0, string in1, string in2);

		/// <remarks/>
		void createPermissionSchemeAsync(string in0, string in1, string in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void deletePermissionScheme(string in0, string in1);

		/// <remarks/>
		void deletePermissionSchemeAsync(string in0, string in1);

		/// <remarks/>
		void deletePermissionSchemeAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("addPermissionToReturn")]
		RemotePermissionScheme addPermissionTo(string in0, RemotePermissionScheme in1, RemotePermission in2, RemoteEntity in3);

		/// <remarks/>
		void addPermissionToAsync(string in0, RemotePermissionScheme in1, RemotePermission in2, RemoteEntity in3);

		/// <remarks/>
		void addPermissionToAsync(string in0, RemotePermissionScheme in1, RemotePermission in2, RemoteEntity in3,
		                          object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("deletePermissionFromReturn")]
		RemotePermissionScheme deletePermissionFrom(string in0, RemotePermissionScheme in1, RemotePermission in2,
		                                            RemoteEntity in3);

		/// <remarks/>
		void deletePermissionFromAsync(string in0, RemotePermissionScheme in1, RemotePermission in2, RemoteEntity in3);

		/// <remarks/>
		void deletePermissionFromAsync(string in0, RemotePermissionScheme in1, RemotePermission in2, RemoteEntity in3,
		                               object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getAllPermissionsReturn")]
		RemotePermission[] getAllPermissions(string in0);

		/// <remarks/>
		void getAllPermissionsAsync(string in0);

		/// <remarks/>
		void getAllPermissionsAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getIssueCountForFilterReturn")]
		long getIssueCountForFilter(string in0, string in1);

		/// <remarks/>
		void getIssueCountForFilterAsync(string in0, string in1);

		/// <remarks/>
		void getIssueCountForFilterAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getIssuesFromTextSearchReturn")]
		RemoteIssue[] getIssuesFromTextSearch(string in0, string in1);

		/// <remarks/>
		void getIssuesFromTextSearchAsync(string in0, string in1);

		/// <remarks/>
		void getIssuesFromTextSearchAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getIssuesFromTextSearchWithProjectReturn")]
		RemoteIssue[] getIssuesFromTextSearchWithProject(string in0, string[] in1, string in2, int in3);

		/// <remarks/>
		void getIssuesFromTextSearchWithProjectAsync(string in0, string[] in1, string in2, int in3);

		/// <remarks/>
		void getIssuesFromTextSearchWithProjectAsync(string in0, string[] in1, string in2, int in3, object userState);


        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://soap.rpc.jira.atlassian.com",
            ResponseNamespace = "http://localhost:8080//rpc/soap/jirasoapservice-v2")]
        [return: System.Xml.Serialization.SoapElementAttribute("getIssuesFromJqlSearchReturn")]
        RemoteIssue[] getIssuesFromJqlSearch(string in0, string in1, int in2);

        /// <remarks/>
        void getIssuesFromJqlSearchAsync(string in0, string in1, int in2);

        /// <remarks/>
        void getIssuesFromJqlSearchAsync(string in0, string in1, int in2, object userState);


		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void deleteUser(string in0, string in1);

		/// <remarks/>
		void deleteUserAsync(string in0, string in1);

		/// <remarks/>
		void deleteUserAsync(string in0, string in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("updateGroupReturn")]
		RemoteGroup updateGroup(string in0, RemoteGroup in1);

		/// <remarks/>
		void updateGroupAsync(string in0, RemoteGroup in1);

		/// <remarks/>
		void updateGroupAsync(string in0, RemoteGroup in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void deleteGroup(string in0, string in1, string in2);

		/// <remarks/>
		void deleteGroupAsync(string in0, string in1, string in2);

		/// <remarks/>
		void deleteGroupAsync(string in0, string in1, string in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		void refreshCustomFields(string in0);

		/// <remarks/>
		void refreshCustomFieldsAsync(string in0);

		/// <remarks/>
		void refreshCustomFieldsAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getProjectsNoSchemesReturn")]
		RemoteProject[] getProjectsNoSchemes(string in0);

		/// <remarks/>
		void getProjectsNoSchemesAsync(string in0);

		/// <remarks/>
		void getProjectsNoSchemesAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("addVersionReturn")]
		RemoteVersion addVersion(string in0, string in1, RemoteVersion in2);

		/// <remarks/>
		void addVersionAsync(string in0, string in1, RemoteVersion in2);

		/// <remarks/>
		void addVersionAsync(string in0, string in1, RemoteVersion in2, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("createProjectFromObjectReturn")]
		RemoteProject createProjectFromObject(string in0, RemoteProject in1);

		/// <remarks/>
		void createProjectFromObjectAsync(string in0, RemoteProject in1);

		/// <remarks/>
		void createProjectFromObjectAsync(string in0, RemoteProject in1, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getSecuritySchemesReturn")]
		RemoteScheme[] getSecuritySchemes(string in0);

		/// <remarks/>
		void getSecuritySchemesAsync(string in0);

		/// <remarks/>
		void getSecuritySchemesAsync(string in0, object userState);

		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://soap.rpc.jira.atlassian.com",
			ResponseNamespace="http://localhost:8080//rpc/soap/jirasoapservice-v2")]
		[return : System.Xml.Serialization.SoapElementAttribute("getIssuesFromFilterReturn")]
		RemoteIssue[] getIssuesFromFilter(string in0, string in1);

		/// <remarks/>
		void getIssuesFromFilterAsync(string in0, string in1);

		/// <remarks/>
		void getIssuesFromFilterAsync(string in0, string in1, object userState);

		/// <remarks/>
		void CancelAsync(object userState);
	}
}
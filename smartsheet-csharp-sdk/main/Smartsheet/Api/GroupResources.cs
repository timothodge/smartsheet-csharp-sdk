﻿//    #[license]
//    SmartsheetClient SDK for C#
//    %%
//    Copyright (C) 2014 SmartsheetClient
//    %%
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//        
//            http://www.apache.org/licenses/LICENSE-2.0
//        
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//    %[license]

using System.Collections.Generic;

namespace Smartsheet.Api
{
    using Api.Models;

    /// <summary>
    /// <para>This interface provides methods to access Group resources.</para>
    /// 
    /// <para>Thread Safety: Implementation of this interface must be thread safe.</para>
    /// </summary>
    public interface GroupResources
    {
        /// <summary>
        /// <para>List all Users.</para>
        /// <para>It mirrors to the following Smartsheet REST API method: GET /groups</para>
        /// <remarks>This operation supports pagination of results. For more information, see Paging.</remarks>
        /// </summary>
        /// <param name="paging"> the pagination</param>
        /// <returns> the list of all Users </returns>
        /// <exception cref="System.InvalidOperationException"> if any argument is null or empty string </exception>
        /// <exception cref="InvalidRequestException"> if there is any problem with the REST API request </exception>
        /// <exception cref="AuthorizationException"> if there is any problem with  the REST API authorization (access token) </exception>
        /// <exception cref="ResourceNotFoundException"> if the resource cannot be found </exception>
        /// <exception cref="ServiceUnavailableException"> if the REST API service is not available (possibly due to rate limiting) </exception>
        /// <exception cref="SmartsheetException"> if there is any other error during the operation </exception>
        PaginatedResult<Group> ListGroups(PaginationParameters? paging);

        /// <summary>
        /// <para>Creates a new Group.</para>
        /// <para>It mirrors to the following Smartsheet REST API method: POST /groups</para>
        /// <remarks>This operation is only available to group administrators and system administrators.</remarks>
        /// </summary>
        /// <param name="group"> the group object </param>
        /// <returns> the created group </returns>
        /// <exception cref="System.InvalidOperationException"> if any argument is null or empty string </exception>
        /// <exception cref="InvalidRequestException"> if there is any problem with the REST API request </exception>
        /// <exception cref="AuthorizationException"> if there is any problem with  the REST API authorization (access token) </exception>
        /// <exception cref="ResourceNotFoundException"> if the resource cannot be found </exception>
        /// <exception cref="ServiceUnavailableException"> if the REST API service is not available (possibly due to rate limiting) </exception>
        /// <exception cref="SmartsheetException"> if there is any other error during the operation </exception>
        Group CreateGroup(Group group);

        /// <summary>
        /// <para>Gets the Group specified in the URL.</para>
        /// <para>It mirrors to the following Smartsheet REST API method: GET /groups/{groupId}</para>
        /// </summary>
        /// <returns> Group object that includes the list of GroupMembers </returns>
        /// <exception cref="System.InvalidOperationException"> if any argument is null or empty string </exception>
        /// <exception cref="InvalidRequestException"> if there is any problem with the REST API request </exception>
        /// <exception cref="AuthorizationException"> if there is any problem with  the REST API authorization (access token) </exception>
        /// <exception cref="ResourceNotFoundException"> if the resource cannot be found </exception>
        /// <exception cref="ServiceUnavailableException"> if the REST API service is not available (possibly due to rate limiting) </exception>
        /// <exception cref="SmartsheetException"> if there is any other error during the operation </exception>
        Group GetGroup(long groupId);

        /// <summary>
        /// <para>Updates the Group specified in the URL.</para>
        /// <para>It mirrors to the following Smartsheet REST API method: PUT /groups/{groupId}</para>
        /// <remarks>This operation is only available to group administrators and system administrators.</remarks>
        /// </summary>
        /// <param name="group"> the group to update </param>
        /// <returns> the updated user </returns>
        /// <exception cref="System.InvalidOperationException"> if any argument is null or empty string </exception>
        /// <exception cref="InvalidRequestException"> if there is any problem with the REST API request </exception>
        /// <exception cref="AuthorizationException"> if there is any problem with  the REST API authorization (access token) </exception>
        /// <exception cref="ResourceNotFoundException"> if the resource cannot be found </exception>
        /// <exception cref="ServiceUnavailableException"> if the REST API service is not available (possibly due to rate limiting) </exception>
        /// <exception cref="SmartsheetException"> if there is any other error during the operation </exception>
        Group UpdateGroup(Group group);

        /// <summary>
        /// <para>Deletes the Group specified in the URL.</para>
        /// <remarks>This operation is only available to system administrators.</remarks>
        /// <para>It mirrors to the following Smartsheet REST API method: DELETE /groups/{groupId}</para>
        /// </summary>
        /// <param name="groupId"> the Id of the group </param>
        /// <exception cref="System.InvalidOperationException"> if any argument is null or empty string </exception>
        /// <exception cref="InvalidRequestException"> if there is any problem with the REST API request </exception>
        /// <exception cref="AuthorizationException"> if there is any problem with  the REST API authorization (access token) </exception>
        /// <exception cref="ResourceNotFoundException"> if the resource cannot be found </exception>
        /// <exception cref="ServiceUnavailableException"> if the REST API service is not available (possibly due to rate limiting) </exception>
        /// <exception cref="SmartsheetException"> if there is any other error during the operation </exception>
        void DeleteGroup(long groupId);

        /// <summary>
        /// <para>Adds one or more members to a Group.</para>
        /// <remarks><para>If called with a single GroupMember object, and that group member already exists, error code 1129 will be returned.
        /// If called with an array of GroupMember objects any users specified in the array that are already group members will be ignored and omitted from the response.</para>
        /// <para>This operation is only available to group administrators and system administrators.</para>
        /// <para>This operation is asynchronous, meaning the users may not yet have sharing access to sheets for a period of time after this operation returns.
        /// For small groups with limited sharing, the operation should complete quickly (within a few seconds).
        /// For large groups with many shares, this operation could possibly take more than a minute to complete.</para></remarks>
        /// <para>It mirrors to the following Smartsheet REST API method: POST /groups/{groupId}/members</para>
        /// </summary>
        /// <param name="groupId"> the Id of the group </param>
        /// <param name="groupMembers"> array of Group Member objects </param>
        /// <returns> the members added to the group </returns>
        /// <exception cref="System.InvalidOperationException"> if any argument is null or empty string </exception>
        /// <exception cref="InvalidRequestException"> if there is any problem with the REST API request </exception>
        /// <exception cref="AuthorizationException"> if there is any problem with  the REST API authorization (access token) </exception>
        /// <exception cref="ResourceNotFoundException"> if the resource cannot be found </exception>
        /// <exception cref="ServiceUnavailableException"> if the REST API service is not available (possibly due to rate limiting) </exception>
        /// <exception cref="SmartsheetException"> if there is any other error during the operation </exception>
        IList<GroupMember> AddGroupMembers(long groupId, IEnumerable<GroupMember> groupMembers);

        /// <summary>
        /// <para>Removes a member from a Group.</para>
        /// <remarks><para>This operation is only available to group administrators and system administrators.</para>
        /// <para>This operation is asynchronous, meaning group members may retain their sharing access for a brief period of time after the call returns.
        /// For small groups with limited sharing, the operation should complete quickly (within a few seconds).
        /// For large groups with many shares, this operation could possibly take more than a minute to complete.</para></remarks>
        /// <para>It mirrors to the following Smartsheet REST API method: DELETE /groups/{groupId}/members/{userId}</para>
        /// </summary>
        /// <param name="groupId"> the Id of the group </param>
        /// <param name="userId"> the Id of the user </param>
        /// <exception cref="System.InvalidOperationException"> if any argument is null or empty string </exception>
        /// <exception cref="InvalidRequestException"> if there is any problem with the REST API request </exception>
        /// <exception cref="AuthorizationException"> if there is any problem with  the REST API authorization (access token) </exception>
        /// <exception cref="ResourceNotFoundException"> if the resource cannot be found </exception>
        /// <exception cref="ServiceUnavailableException"> if the REST API service is not available (possibly due to rate limiting) </exception>
        /// <exception cref="SmartsheetException"> if there is any other error during the operation </exception>
        void RemoveGroupMember(long groupId, long userId);
    }
}
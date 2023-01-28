    public class FriendService : IFriendService
    {
        IDataProvider _data = null;

        public FriendService(IDataProvider data) { _data = data; }

        public int Add(FriendAddRequest model, int userId)
        {
            int id = 0;

            string procName = "[dbo].[Friends_Insert]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col, userId);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                });
            return id;
        }

        public void Update(FriendUpdateRequest model, int userId)
        {
            string procName = "[dbo].[Friends_Update]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col, userId);

                    col.AddWithValue("@Id", model.Id);
                },
                returnParameters: null);
        }

        public Friend Get(int id)
        {
            string procName = "[dbo].[Friends_SelectById]";

            Friend friend = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                friend = MapSingleFriend(reader);
            });
            return friend;
        }

        public List<Friend> GetAll()
        {
            List<Friend> list = null;

            string procName = "[dbo].[Friends_SelectAll]";

            _data.ExecuteCmd(procName, inputParamMapper: null
            , singleRecordMapper: delegate (IDataReader reader, short set)
            {
                Friend aFriend = MapSingleFriend(reader);

                if (list == null)
                {
                    list = new List<Friend>();
                }

                list.Add(aFriend);
            });
            return list;
        }

        public void Delete(int id)
        {
            string procName = "[dbo].[Friends_Delete]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                },
                returnParameters: null
                );
        }

        private static Friend MapSingleFriend(IDataReader reader)
        {
            Friend aFriend = new Friend();

            int startingIndex = 0;

            aFriend.Id = reader.GetSafeInt32(startingIndex++);
            aFriend.Title = reader.GetSafeString(startingIndex++);
            aFriend.Bio = reader.GetSafeString(startingIndex++);
            aFriend.Summary = reader.GetSafeString(startingIndex++);
            aFriend.Headline = reader.GetSafeString(startingIndex++);
            aFriend.Slug = reader.GetSafeString(startingIndex++);
            aFriend.StatusId = reader.GetSafeInt32(startingIndex++);
            aFriend.PrimaryImageUrl = reader.GetSafeString(startingIndex++);
            aFriend.UserId = reader.GetSafeInt32(startingIndex++);
            aFriend.DateCreated = reader.GetSafeUtcDateTime(startingIndex++);
            aFriend.DateModified = reader.GetSafeUtcDateTime(startingIndex++);
            return aFriend;
        }

        private static void AddCommonParams(FriendAddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@Title", model.Title);
            col.AddWithValue("@Bio", model.Bio);
            col.AddWithValue("@Summary", model.Summary);
            col.AddWithValue("@Headline", model.Headline);
            col.AddWithValue("@Slug", model.Slug);
            col.AddWithValue("@StatusId", model.StatusId);
            col.AddWithValue("@PrimaryImageUrl", model.PrimaryImageUrl);
            col.AddWithValue("@UserId", userId);
        }

        //FriendV2
        public int AddV2(FriendV2AddRequest model, int userId)
        {
            int id = 0;

            string procName = "[dbo].[Friends_InsertV2]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParamsV2(model, col, userId);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                });
            return id;
        }

        public void UpdateV2(FriendV2UpdateRequest model, int userId)
        {
            string procName = "[dbo].[Friends_UpdateV2]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParamsV2(model, col, userId);

                    col.AddWithValue("@Id", model.Id);
                },
                returnParameters: null);
        }

        public FriendV2 GetV2(int id)
        {
            string procName = "[dbo].[Friends_SelectByIdV2]";

            FriendV2 friend = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                friend = MapSingleFriendV2(reader, ref startingIndex);
            });
            return friend;
        }

        public List<FriendV2> GetAllV2()
        {
            List<FriendV2> list = null;

            string procName = "[dbo].[Friends_SelectAllV2]";

            _data.ExecuteCmd(procName, inputParamMapper: null
            , singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                FriendV2 aFriend = MapSingleFriendV2(reader, ref startingIndex);

                if (list == null)
                {
                    list = new List<FriendV2>();
                }

                list.Add(aFriend);
            });
            return list;
        }

        public Paged<FriendV2> PaginationV2(int pageIndex, int pageSize)
        {
            Paged<FriendV2> pagedList = null;
            List<FriendV2> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[Friends_PaginationV2]",
                (param) =>

                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    FriendV2 friend = MapSingleFriendV2(reader, ref startingIndex);
                    totalCount = reader.GetSafeInt32(startingIndex++);

                    if (list == null)
                    {
                        list = new List<FriendV2>();
                    }

                    list.Add(friend);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<FriendV2>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Paged<FriendV2> SearchPaginatedV2(int pageIndex, int pageSize, string query)
        {
            Paged<FriendV2> pagedList = null;
            List<FriendV2> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[Friends_Search_PaginationV2]",
                (param) =>

                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@Query", query);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;

                    FriendV2 friend = MapSingleFriendV2(reader, ref startingIndex);
                    if(totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex++);
                    }

                    if (list == null)
                    {
                        list = new List<FriendV2>();
                    }

                    list.Add(friend);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<FriendV2>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public void DeleteV2(int id)
        {
            string procName = "[dbo].[Friends_DeleteV2]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                },
                returnParameters: null
                );
        }

        private static FriendV2 MapSingleFriendV2(IDataReader reader, ref int startingIndex)
        {
            FriendV2 aFriend = new FriendV2();
            aFriend.PrimaryImage = new Image();


            aFriend.Id = reader.GetSafeInt32(startingIndex++);
            aFriend.Title = reader.GetSafeString(startingIndex++);
            aFriend.Bio = reader.GetSafeString(startingIndex++);
            aFriend.Summary = reader.GetSafeString(startingIndex++);
            aFriend.Headline = reader.GetSafeString(startingIndex++);
            aFriend.Slug = reader.GetSafeString(startingIndex++);
            aFriend.StatusId = reader.GetSafeInt32(startingIndex++);
            aFriend.PrimaryImage.Id = reader.GetSafeInt32(startingIndex++);
            aFriend.PrimaryImage.TypeId = reader.GetSafeInt32(startingIndex++);
            aFriend.PrimaryImage.Url = reader.GetSafeString(startingIndex++);
            aFriend.UserId = reader.GetSafeInt32(startingIndex++);
            aFriend.DateCreated = reader.GetSafeUtcDateTime(startingIndex++);
            aFriend.DateModified = reader.GetSafeUtcDateTime(startingIndex++);
            return aFriend;
        }

        private static void AddCommonParamsV2(FriendV2AddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@Title", model.Title);
            col.AddWithValue("@Bio", model.Bio);
            col.AddWithValue("@Summary", model.Summary);
            col.AddWithValue("@Headline", model.Headline);
            col.AddWithValue("@Slug", model.Slug);
            col.AddWithValue("@StatusId", model.StatusId);
            col.AddWithValue("@ImageTypeId", model.ImageTypeId);
            col.AddWithValue("@ImageUrl", model.ImageUrl);
            col.AddWithValue("@UserId", userId);
        }

        //FriendV3
        public FriendV3 GetV3(int id)
        {
            string procName = "[dbo].[Friends_SelectByIdV3]";

            FriendV3 friend = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                friend = MapSingleFriendV3(reader, ref startingIndex);
            });
            return friend;
        }

        public List<FriendV3> GetAllV3()
        {
            List<FriendV3> list = null;

            string procName = "[dbo].[Friends_SelectAllV3]";

            _data.ExecuteCmd(procName, inputParamMapper: null
            , singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                FriendV3 aFriend = MapSingleFriendV3(reader, ref startingIndex);

                if (list == null)
                {
                    list = new List<FriendV3>();
                }

                list.Add(aFriend);
            });
            return list;
        }

        public Paged<FriendV3> PaginationV3(int pageIndex, int pageSize)
        {
            Paged<FriendV3> pagedList = null;
            List<FriendV3> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[Friends_PaginationV3]",
                (param) =>

                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    FriendV3 friend = MapSingleFriendV3(reader, ref startingIndex);
                    totalCount = reader.GetSafeInt32(startingIndex++);

                    if (list == null)
                    {
                        list = new List<FriendV3>();
                    }

                    list.Add(friend);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<FriendV3>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Paged<FriendV3> SearchPaginatedV3(int pageIndex, int pageSize, string query)
        {
            Paged<FriendV3> pagedList = null;
            List<FriendV3> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[Friends_Search_PaginationV3]",
                (param) =>

                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@Query", query);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;

                    FriendV3 friend = MapSingleFriendV3(reader, ref startingIndex);
                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex++);
                    }

                    if (list == null)
                    {
                        list = new List<FriendV3>();
                    }

                    list.Add(friend);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<FriendV3>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public int AddV3(FriendV3AddRequest model, int userId)
        {
            int id = 0;

            string procName = "[dbo].[Friends_InsertV3]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParamsV3(model, col, userId);
                    col.AddWithValue("@newSkill", MapSkillsToTable(model.Skills));

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                });
            return id;
        }

        private DataTable MapSkillsToTable(List<string> skillsToMap)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
 

            foreach (string skill in skillsToMap)
            {
                DataRow dr = dt.NewRow();
                int startingIndex = 0;

                dr.SetField(startingIndex++, skill);

                dt.Rows.Add(dr);
            }

            return dt;
        }

        private static void AddCommonParamsV3(FriendV3AddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@Title", model.Title);
            col.AddWithValue("@Bio", model.Bio);
            col.AddWithValue("@Summary", model.Summary);
            col.AddWithValue("@Headline", model.Headline);
            col.AddWithValue("@Slug", model.Slug);
            col.AddWithValue("@StatusId", model.StatusId);
            col.AddWithValue("@ImageTypeId", model.ImageTypeId);
            col.AddWithValue("@ImageUrl", model.ImageUrl);
            col.AddWithValue("@UserId", userId);
        }

        private static FriendV3 MapSingleFriendV3(IDataReader reader, ref int startingIndex)
        {
            FriendV3 aFriend = new FriendV3();
            aFriend.PrimaryImage = new Image();

            aFriend.Id = reader.GetSafeInt32(startingIndex++);
            aFriend.Title = reader.GetSafeString(startingIndex++);
            aFriend.Bio = reader.GetSafeString(startingIndex++);
            aFriend.Summary = reader.GetSafeString(startingIndex++);
            aFriend.Headline = reader.GetSafeString(startingIndex++);
            aFriend.Slug = reader.GetSafeString(startingIndex++);
            aFriend.StatusId = reader.GetSafeInt32(startingIndex++);
            aFriend.PrimaryImage.Id = reader.GetSafeInt32(startingIndex++);
            aFriend.PrimaryImage.TypeId = reader.GetSafeInt32(startingIndex++);
            aFriend.PrimaryImage.Url = reader.GetSafeString(startingIndex++);
            aFriend.Skills = reader.DeserializeObject<List<Skill>>(startingIndex++);
            aFriend.UserId = reader.GetSafeInt32(startingIndex++);
            aFriend.DateCreated = reader.GetSafeUtcDateTime(startingIndex++);
            aFriend.DateModified = reader.GetSafeUtcDateTime(startingIndex++);
            return aFriend;
        }

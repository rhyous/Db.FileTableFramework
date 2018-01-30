using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rhyous.Db.FileTableFramework
{
    public static class FileExtensions
    {
        public static List<File> ToFiles(this SqlDataReader reader)
        {
            var files = new List<File>();
            while (reader.Read())
                files.Add(reader.ToFile());
            return files;
        }
        public static File ToFile(this SqlDataReader reader)
        {
            return new File()
            {
                stream_id = (Guid)reader["stream_id"],
                file_stream = reader["file_stream"] == DBNull.Value ? null : (byte[])reader["file_stream"],
                name = reader["name"].ToString(),
                path_locator = (SqlHierarchyId)reader["path_locator"],
                parent_path_locator = reader["parent_path_locator"] == DBNull.Value ? SqlHierarchyId.Null : (SqlHierarchyId)reader["parent_path_locator"],
                file_type = reader["file_type"] == DBNull.Value ? null : reader["file_type"].ToString(),
                cached_file_size = reader["cached_file_size"] == DBNull.Value ? new long?() : (long)reader["cached_file_size"],
                creation_time = (DateTimeOffset)reader["creation_time"],
                last_write_time = (DateTimeOffset)reader["last_write_time"],
                last_access_time = reader["last_access_time"] == DBNull.Value ? new DateTimeOffset?() : (DateTimeOffset)reader["last_access_time"],
                is_directory = (bool)reader["is_directory"],
                is_offline = (bool)reader["is_offline"],
                is_hidden = (bool)reader["is_hidden"],
                is_readonly = (bool)reader["is_readonly"],
                is_archive = (bool)reader["is_archive"],
                is_system = (bool)reader["is_system"],
                is_temporary = (bool)reader["is_temporary"]
            };
        }

        public static void FillFileRow(this File file, out Guid stream_id,
                                       out byte[] file_stream,
                                       out string name,
                                       out SqlHierarchyId path_locator,
                                       out SqlHierarchyId parent_path_locator,
                                       out string file_type,
                                       out long? cached_file_size,
                                       out DateTimeOffset creation_time,
                                       out DateTimeOffset last_write_time,
                                       out DateTimeOffset? last_access_time,
                                       out bool is_directory,
                                       out bool is_offline,
                                       out bool is_hidden,
                                       out bool is_readonly,
                                       out bool is_archive,
                                       out bool is_system,
                                       out bool is_temporary
            )
        {
            if (file == null)
                throw new Exception();
            stream_id = file.stream_id;
            file_stream = file.file_stream;
            name = file.name;
            path_locator = file.path_locator;
            parent_path_locator = file.parent_path_locator;
            file_type = file.file_type;
            cached_file_size = file.cached_file_size;
            creation_time = file.creation_time;
            last_write_time = file.last_write_time;
            last_access_time = file.last_access_time;
            is_directory = file.is_directory;
            is_offline = file.is_offline;
            is_hidden = file.is_hidden;
            is_readonly = file.is_readonly;
            is_archive = file.is_archive;
            is_system = file.is_system;
            is_temporary = file.is_temporary;
        }
    }
}

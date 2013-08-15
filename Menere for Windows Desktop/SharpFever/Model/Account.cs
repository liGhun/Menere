using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using SharpFever.Model;
using Newtonsoft.Json;


namespace SharpFever.Model
{
    public class Account
    {
        public string base_url { get; set; }
        public ulong last_refreshed_on_time { get; set; }

        public FeverResponse check_credentials() {
            return get_general_request("");
        }

        public FeverResponse get_feeds()
        {
            return get_general_request("&feeds");
        }

        public FeverResponse get_items(int since_id = -1, int max_id = -1, List<uint> with_ids = null)
        {
            string parameter = "&items";
            if (since_id >= 0)
            {
                parameter += "&since_id=" + since_id.ToString();
            }

            if (max_id >= 0)
            {
                parameter += "&max_id=" + max_id.ToString();
            }

            if (with_ids != null)
            {
                if (with_ids.Count > 0)
                {
                    parameter += "&with_ids=" + string.Join(",", with_ids);
                }
            }
            FeverResponse response = get_general_request(parameter);

            if (response != null)
            {
                if (response.auth)
                {
                    last_refreshed_on_time = response.last_refreshed_on_time;
                }
            }

            return response;
        }

        public FeverResponse get_groups()
        {
            return get_general_request("&groups");
        }

        public FeverResponse get_favicons()
        {
            return get_general_request("&favicons");
        }

        public FeverResponse get_links(int offset = -1, int range = -1, int page = -1)
        {
            string parameter = "&links";
            
            if(offset >= 0) {
                parameter += "&offset=" + offset;
            }

            if(range >= 0) {
                parameter += "&range=" + range;
            }

            if(page >= 0) {
                parameter += "&page=" + page;
            }
            
            return get_general_request(parameter);
        }

        public FeverResponse get_unread_item_ids()
        {
            return get_general_request("&unread_item_ids");
        }

        public FeverResponse get_saved_item_ids()
        {
            return get_general_request("&saved_item_ids");
        }

        private FeverResponse get_general_request(string parameters, Dictionary<string, string> additional_post_data = null)
        {
            string requestUrl = base_url + parameters;
            Dictionary<string, string> post_data = additional_post_data;
            if (post_data == null)
            {
                post_data = new Dictionary<string, string>();
            }
            post_data.Add("api_key", api_key);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            Helper.Response response = Helper.SendPostRequest(
                    requestUrl,
                    post_data
                    );

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Error += delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                throw args.ErrorContext.Error;
            };
            FeverResponse feverResponse = new FeverResponse();
            feverResponse = JsonConvert.DeserializeObject<FeverResponse>(response.Content, settings);

            return feverResponse;
        }


        public string email { get; set; }
        public string password { get; set; }
        public string api_key
        {
            get
            {
                if (!string.IsNullOrEmpty(stored_api_key))
                {
                    return stored_api_key;
                }
                else
                {
                    MD5 md5Hash = MD5.Create();
                    return GetMd5Hash(md5Hash, email + ":" + password);
                }

            }
            set
            {
                stored_api_key = value;
            }
        }
        public string stored_api_key { get; set; }

        public bool mark_item_as_read(Item item) {
            if(item != null) 
            {
                return mark_item_as_read(item.id);
            }
            else
            {
                return false;
            }
        }

        public bool mark_item_as_read(uint id) {
             Dictionary<string, string> parameters = new Dictionary<string,string>();
            parameters.Add("mark","item");
            parameters.Add("as","read");
            parameters.Add("id", id.ToString());
            FeverResponse response = get_general_request("", parameters);
            if (response == null)
            {
                response = new FeverResponse();
                response.auth = false;
            }
            return response.auth;
        }

        public bool mark_item_as_saved(Item item)
        {
            if (item != null)
            {
                return mark_item_as_saved(item.id);
            }
            else
            {
                return false;
            }
        }

        public bool mark_item_as_saved(uint id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mark", "item");
            parameters.Add("as", "saved");
            parameters.Add("id", id.ToString());
            FeverResponse response = get_general_request("", parameters);
            return response.auth;
        }

        public bool mark_item_as_unsaved(Item item)
        {
            if (item != null)
            {
                return mark_item_as_unsaved(item.id);
            }
            else
            {
                return false;
            }
        }

        public bool mark_item_as_unsaved(uint id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mark", "item");
            parameters.Add("as", "unsaved");
            parameters.Add("id", id.ToString());
            FeverResponse response = get_general_request("", parameters);
            return response.auth;
        }

        public bool mark_feed_as_read(Feed feed, ulong max_timestamp = 4711)
        {
            if (feed != null)
            {
                return mark_feed_as_read(feed.id, max_timestamp);
            }
            else
            {
                return false;
            }
        }

        public bool mark_feed_as_read(uint id, ulong max_timestamp = 4711)
        {
            if(max_timestamp == 4711) {
                max_timestamp = last_refreshed_on_time;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mark", "feed");
            parameters.Add("as", "read");
            parameters.Add("id", id.ToString());
            parameters.Add("before",max_timestamp.ToString());
            FeverResponse response = get_general_request("", parameters);
            return response.auth;
        }

        public bool mark_group_as_read(Group group, ulong max_timestamp = 4711)
        {
            if (group != null)
            {
                return mark_group_as_read(group.id, max_timestamp);
            }
            else
            {
                return false;
            }
        }

        public bool mark_group_as_read(uint id, ulong max_timestamp = 4711)
        {
            if (max_timestamp == 4711)
            {
                max_timestamp = last_refreshed_on_time;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mark", "group");
            parameters.Add("as", "read");
            parameters.Add("id", id.ToString());
            parameters.Add("before", max_timestamp.ToString());
            FeverResponse response = get_general_request("", parameters);
            return response.auth;
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}

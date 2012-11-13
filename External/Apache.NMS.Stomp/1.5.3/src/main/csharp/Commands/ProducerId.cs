/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace Apache.NMS.Stomp.Commands
{
    public class ProducerId : BaseDataStructure
    {
        private SessionId parentId;

        private string key = null;

        string connectionId;
        long value;
        long sessionId;

        public ProducerId()
        {
        }

        public ProducerId(SessionId sessionId, long consumerId)
        {
            this.connectionId = sessionId.ConnectionId;
            this.sessionId = sessionId.Value;
            this.value = consumerId;
        }

        public ProducerId(string producerKey)
        {
            // Store the original.
            this.key = producerKey;

            // Try and get back the AMQ version of the data.
            int idx = producerKey.LastIndexOf(':');
            if(idx >= 0)
            {
                try
                {
                    this.Value = Int32.Parse(producerKey.Substring(idx + 1));
                    producerKey = producerKey.Substring(0, idx);
                    idx = producerKey.LastIndexOf(':');
                    if(idx >= 0)
                    {
                        this.SessionId = Int32.Parse(producerKey.Substring(idx + 1));
                        producerKey = producerKey.Substring(0, idx);
                    }
                }
                catch(Exception ex)
                {
                    Tracer.Debug(ex.Message);
                }
            }
            this.ConnectionId = producerKey;
        }

        ///
        /// <summery>
        ///  Get the unique identifier that this object and its own
        ///  Marshaler share.
        /// </summery>
        ///
        public override byte GetDataStructureType()
        {
            return DataStructureTypes.ProducerIdType;
        }

        ///
        /// <summery>
        ///  Returns a string containing the information for this DataStructure
        ///  such as its type and value of its elements.
        /// </summery>
        ///
        public override string ToString()
        {
            if( this.key == null )
            {
                this.key = ConnectionId + ":" + SessionId + ":" + Value;
            }

            return this.key;
        }

        public SessionId ParentId
        {
            get
            {
                 if(this.parentId == null)
                 {
                    this.parentId = new SessionId(this);
                 }
                 return this.parentId;
            }
        }

        public string ConnectionId
        {
            get { return connectionId; }
            set { this.connectionId = value; }
        }

        public long Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public long SessionId
        {
            get { return sessionId; }
            set { this.sessionId = value; }
        }

        public override int GetHashCode()
        {
            int answer = 0;

            answer = (answer * 37) + HashCode(ConnectionId);
            answer = (answer * 37) + HashCode(Value);
            answer = (answer * 37) + HashCode(SessionId);

            return answer;
        }

        public override bool Equals(object that)
        {
            if(that is ProducerId)
            {
                return Equals((ProducerId) that);
            }
            return false;
        }

        public virtual bool Equals(ProducerId that)
        {
            if(!Equals(this.ConnectionId, that.ConnectionId))
            {
                return false;
            }
            if(!Equals(this.Value, that.Value))
            {
                return false;
            }
            if(!Equals(this.SessionId, that.SessionId))
            {
                return false;
            }

            return true;
        }
    };
}


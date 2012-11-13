/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

using Apache.NMS.Stomp.Protocol;

namespace Apache.NMS.Stomp.Commands
{
    public class TextMessage : Message, ITextMessage
    {
        private String text;

        public TextMessage()
        {
        }

        public TextMessage(String text)
        {
            this.Text = text;
        }

        public override string ToString()
        {
            string text = this.Text;

            if(text != null && text.Length > 63)
            {
                text = text.Substring(0, 45) + "..." + text.Substring(text.Length - 12);
            }
            return base.ToString() + " Text = " + (text ?? "null");
        }

        public override void ClearBody()
        {
            base.ClearBody();
            this.text = null;
        }

        public override byte GetDataStructureType()
        {
            return DataStructureTypes.TextMessageType;
        }

        // Properties

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                FailIfReadOnlyBody();
                this.text = value;
                this.Content = null;
            }
        }

        public override void BeforeMarshall(StompWireFormat wireFormat)
        {
            base.BeforeMarshall(wireFormat);

            if(this.Content == null && text != null)
            {
                this.Content = wireFormat.Encoder.GetBytes(this.text);
                this.text = null;
            }
        }

        public override int Size()
        {
            if(this.Content == null && text != null)
            {
                int size = DEFAULT_MINIMUM_MESSAGE_SIZE;

                if(MarshalledProperties != null)
                {
                    size += MarshalledProperties.Length;
                }

                return (size += this.text.Length * 2);
            }

            return base.Size();
        }
    }
}


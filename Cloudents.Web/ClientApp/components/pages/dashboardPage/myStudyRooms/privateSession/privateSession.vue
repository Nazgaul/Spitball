<template>
    <div class="privateLesson">
        <v-form class="form">
            <div class="d-flex">
               <v-text-field
                  :rules="[rules.required]"
                  v-model="roomName"
                  class="roomName"
                  height="50"
                  dense
                  outlined
                  :label="$t('dashboardPage_create_room_placeholder')"
                  :placeholder="$t('dashboardPage_create_room_label')"
               >
               </v-text-field>
               <v-text-field 
                  class="pl-4 roomPrice"
                  outlined
                  height="50"
                  dense
                  :rules="[rules.required,rules.minimum]"
                  v-model="price" type="number"
                  :label="$t('becomeTutor_placeholder_price', {'0' : getSymbol})"
                  :placeholder="$t('becomeTutor_placeholder_price', {'0' : getSymbol})"
               >
               </v-text-field>
            </div>

            
            <div class="createStudyRoomDialog-list mb-6">
               <div class="listTitle" v-t="'dashboardPage_invite_students'"></div>
               <v-list flat class="list-followers">
                  <v-list-item-group>
                     <v-list-item v-for="(item, index) in myFollowers" :key="index" @click="addSelectedUser(item)" :class="[{'dark-line': index % 2}]">
                        <template v-slot:default="{}">
                           <v-list-item-avatar>
                              <UserAvatar :size="'40'" :user-name="item.name" :user-id="item.id" :userImageUrl="item.image"/> 
                           </v-list-item-avatar>
                           <v-list-item-content>
                              <v-list-item-title>{{item.name}}</v-list-item-title>
                           </v-list-item-content>
                           <v-list-item-action>
                              <v-checkbox @click.prevent multiple v-model="selected" :value="item" color="#4c59ff" off-icon="sbf-check-box-un" on-icon="sbf-check-box-done"></v-checkbox>
                           </v-list-item-action>
                        </template>
                     </v-list-item>
                  </v-list-item-group>
               </v-list>
            </div>

            <v-btn
                :loading="isLoading"
                @click="createStudyRoom"
                width="150"
                depressed
                height="40"
                color="#4452fc"
                class="white--text"
                rounded
            >
                {{$t('dashboardPage_create_private')}}
            </v-btn>
        </v-form>
    </div>
</template>

<script>
import { validationRules } from '../../../../../services/utilities/formValidationRules.js'

export default {
    name: 'privateLesson',
    data() {
        return {
            isLoading: false,
            selected:[],
            myFollowers:[],
            roomName:'',
            price: 0,
            rules: {
                required: (value) => validationRules.required(value),
                minimum: (value) => validationRules.minVal(value,0),
            },
        }
    },
    computed: {
        getSymbol() {
            let v =  this.$n(1,'currency');
            return v.replace(/\d|[.,]/g,'').trim();
        },
    },
    methods: {
        createStudyRoom(){
            if(!this.$refs.createRoomValidation.validate()) return

            if(!this.isLoading && !this.showErrorAlreadyCreated && !this.showErrorEmpty && !this.showErrorMaxUsers){
                let isBroadcast = this.studyRoomType === 'broadcast'
                
                if(!this.selected.length && !isBroadcast){
                this.showErrorEmpty = true;
                return 
                }
                
                let paramsObj = {
                    name: this.roomName,
                    userId: Array.from(this.selected.map(user=> user.userId)),
                    price: this.price || 0,
                    type: this.studyRoomType,
                }
                
                this.isLoading = true
                let self = this;
                this.$store.dispatch('updateCreateStudyRoom',paramsObj)
                .then(() => {
                    self.isLoading = false;
                    self.$store.commit('setComponent')
                }).catch((error)=>{
                    self.isLoading = false;
                    if(error.response?.status == 409){
                        self.showErrorAlreadyCreated = true;
                    }
                });
            }
        },
    },
    created() {
        this.$store.dispatch('updateFollowersItems').then(()=>{
            this.myFollowers = this.$store.getters.getFollowersItems
        })
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';

.privateLesson {
   .form{
        width: 100%;
        .roomName {
            ::placeholder {
                color: @global-purple;
            }
            :-ms-input-placeholder { /* Internet Explorer 10-11 */
                color: @global-purple;
            }
        }
        .roomPrice {
            flex: 1;
        }
    }
    .createStudyRoomDialog-list{
        width: 100%;
        // height: 320px;
        .listTitle {
            font-size: 18px;
            font-weight: 600;
            color: @global-purple;
        }
        .list-followers{
            max-height: 320px;
            overflow-y: scroll;
            .v-item-group {
                padding-right: 6px;
            }
            .dark-line{
                background: #f5f5f5;
            }
        }
    }
}    
</style>
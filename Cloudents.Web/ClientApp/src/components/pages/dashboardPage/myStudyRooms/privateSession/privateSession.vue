<template>
    <div class="privateLesson">
        <v-row class="pa-0 ma-0" dense>
            <v-col cols="12" sm="8" class="pa-0">
                <v-text-field
                    v-model="roomName"
                    class="roomName"
                    :rules="[rules.required]"
                    placeholder=" "
                    :label="$t('dashboardPage_create_room_placeholder')"
                    color="#304FFE"
                    height="50"
                    outlined
                    dense
                >
                </v-text-field>
            </v-col>
            <v-col cols="12" sm="4" class="pa-0">
                <v-text-field 
                    v-model="myPrice"
                    type="number"
                    class="ps-sm-4 roomPrice"
                    :rules="[rules.required,rules.minimum]"
                    placeholder=" "
                    :label="$t('becomeTutor_placeholder_price', {'0' : getSymbol})"
                    color="#304FFE"
                    height="50"
                    outlined
                    dense
                >
                </v-text-field>
            </v-col>
        </v-row>
        <div class="createStudyRoomDialog-list mb-6" v-if="myFollowers.length">
            <div class="listTitle" v-t="'dashboardPage_invite_students'"></div>
            <v-list flat class="list-followers">
                <v-list-item-group>
                    <v-list-item 
                        v-for="(item, index) in myFollowers"
                        :key="index"
                        @click="addSelectedUser(item)"
                        :class="[{'dark-line': index % 2}]"
                    >
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
        <div v-else class="listTitle">
            {{$t('no_followers')}}
        </div>
    </div>
</template>

<script>
import { validationRules } from '../../../../../services/utilities/formValidationRules.js'

export default {
    name: 'privateLesson',
    props: {
        price: {
            required: true
        },
        currentError: {
            type: String,
        },
    },
    data() {
        return {
            isLoading: false,
            selected:[],
            myFollowers:[],
            roomName:'',
            MAX_PARTICIPANT: 49,
            rules: {
                required: (value) => validationRules.required(value),
                minimum: (value) => validationRules.minVal(value,0),
            },
        }
    },
    watch: {
        roomName(val) {
            if(val && this.currentError) {
                this.$emit('resetErrors')
            }
        }
    },
    computed: {
        myPrice: {
            get() {
                return this.price
            },
            set(price) {
                this.$emit('updatePrice', price)
            }
        },
        getSymbol() {
            // TODO: Currency Change
            let accountUser = this.$store.getters.accountUser
            let v = this.$n(1, {'style':'currency','currency': accountUser?.currencySymbol});
            return v.replace(/\d|[.,]/g,'').trim();
        },
    },
    methods: {
        addSelectedUser(user){
            let idx;
            let isInList = this.selected.some((u,i)=>{
                idx = i;
                return u.userId === user.userId;
            })
            if(isInList){
                this.selected.splice(idx,1);
            } else {
                if(this.selected.length < this.MAX_PARTICIPANT){
                    this.$emit('resetErrors')
                    this.selected.push(user)
                } else {
                    this.$emit('updateError', 'showErrorMaxUsers')
                }
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
@import '../../../../../styles/colors.less';

.privateLesson {
    .roomName {
        ::placeholder {
            color: #bbb !important;
        }
        :-ms-input-placeholder { /* Internet Explorer 10-11 */
            color: #bbb !important;
        }
    }
    .roomPrice {
        flex: 1;
    }
    .listTitle {
        font-size: 18px;
        font-weight: 600;
        color: @global-purple;
        white-space: pre-line;
    }
    .createStudyRoomDialog-list{
        min-height: 320px;
        width: 100%;
        .list-followers {
            overflow-y: scroll;
            max-height: 270px;

            @media (max-width: @screen-xs) {
                max-height: 350px;
            }
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
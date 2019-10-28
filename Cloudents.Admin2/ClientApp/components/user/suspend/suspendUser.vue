<template>
    <form class="suspend-container">
        <h1>Suspend User</h1>
        <div class="suspend-input-container">
            <v-text-field solo type="text" class="user-id-input" :rules="[rulesId.required]" placeholder="Insert user id..." v-model="ids"/>
        </div>
        <div class="suspend-input-container">
            <v-text-field solo type="text" class="user-id-input" :rules="[rulesReason.required]"  placeholder="reason" v-model="reason"/>
        </div>
        <div class="suspend-button-container">
            <v-btn  :loading="suspendLoading" :disabled="!ids || !reason" color="red" @click.prevent="actionUser(false)" :class="{'lock': lock}">Suspend</v-btn>
        </div>
        <div v-if="showSuspendedDetails" class="suspended-user-container">
            <h3>Email: {{suspendedMail}}</h3>
        </div>
    </form>
</template>

<script>
import {suspendUser, releaseUser} from './suspendUserService';
import {mapActions, mapGetters} from 'vuex';

export default {
    data(){
        return{
            rulesReason: {
                required: value => !!value || 'Required.',

            },
            rulesId: {
                required: value => !!value || 'Required.',
            },
            serverIds: [],
            ids: this.userIds,
            showSuspendedDetails: false,
            suspendedMail: null,
            lock: false,
            suspendLoading: false,
            releaseLoading: false,
            reason: null
        }
    },
    props: {
        userIds: null
    },
    computed: {
        ...mapGetters(['userInfo'])
    },
    methods:{
        ...mapActions([
            "setSuspendDialogState",
            "setUserCurrentStatus"
        ]),
        actionUser:function(unsuspendUser){
            if(!this.ids){
                this.$toaster.error("Please Insert A user ID");
                return;
            }
            if (this.reason == null || !this.reason.trim())
            {
                 this.$toaster.error("Please Insert A Reason");
                return;
            }
            this.serverIds = [];
            this.ids = '' + this.ids;
            this.ids.split(',').forEach(id=>{
                let num = parseInt(id.trim());
                if(!!num){
                    return this.serverIds.push(num);
                }  
            });
            
            this.lock = true;
            if(!!unsuspendUser){
                this.releaseLoading = true;
                releaseUser(this.serverIds).then((email)=>{
                    this.$toaster.success(`user got released`);
                    this.releaseLoading = false;
                    this.reason = null;
                    this.ids = null;
                }, (err)=>{
                    this.$toaster.error(`ERROR: failed to realse user`);
                    console.log(err)
                }).finally(()=>{
                    this.setSuspendDialogState(false);
                    this.setUserCurrentStatus(false)
                    this.lock = false;
                    this.releaseLoading = false;
                })
            }else{
                if(!this.reason){
                    this.$toaster.error("Please Insert A Reason");
                    this.lock = false;
                    return;
                }
                let self = this
                this.suspendLoading = true;
                suspendUser(this.serverIds, this.reason).then((email)=>{
                    this.$toaster.success(`user got suspended, email is: ${self.userInfo.email.value}`)
                    this.showSuspendedDetails = true;
                    this.suspendedMail = self.userInfo.email.value;
                    this.suspendLoading = false;
                    this.reason = null;
                    this.ids = null;
                }, (err)=>{
                    this.$toaster.error(`ERROR: failed to suspend user`);
                    console.log(err)
                }).finally(()=>{
                    this.setSuspendDialogState(false);
                    this.setUserCurrentStatus(true);

                    this.lock = false;
                    this.suspendLoading = false;
                })
            }
            
        }
    }
}
</script>

<style lang="less" scoped>
.suspend-container{
    .suspend-input-container{
        justify-content: center;
        align-items: center;
        display: flex;
        flex-direction: column;
        .user-id-input{
            border: none;
            outline: none;
            border-radius: 25px;
            /*height: 15px;*/
            margin-top: 5px;
            padding: 10px;
            width: 345px;
        }
    }
    .suspend-checkbox-container{
        margin-top: 15px;
    }
    .suspend-button-container{
            margin-top: 15px;
            button{
                /*cursor: pointer;*/
                /*border: none;*/
                /*outline: none;*/
                /*background-color: #f35a5a;*/
                /*border-radius: 25px;*/
                /*height: 25px;*/
                /*width: 80px;*/
                /*color: #810000;*/
                /*font-weight: 600;*/
                &.lock{
                    background-color: #d1d1d1;
                    color: #a19d9d;         
                    pointer-events: none;
                }
            }
        }
}
</style>

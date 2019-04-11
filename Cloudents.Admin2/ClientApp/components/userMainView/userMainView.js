import { mapActions, mapGetters } from 'vuex';
import { releaseUser } from '../user/suspend/suspendUserService';
import userTokens from './helpers/sendTokens.vue';
import userSuspend from '../user/suspend/suspendUser.vue';
import userCashout from '../user/cashout/cashoutUser.vue';


export default {
    name: "userMainView",
    components: {
        userTokens,
        userCashout,
        userSuspend
    },
    props: {
        userId: {}
    },
    data() {
        return {
            userIdentifier: '',
            suspendedUser: false,
            filters: [
                {name: 'Accepted', value: 'ok'},
                {name: 'Pending', value: 'pending'},
                {name: 'Deleted', value: 'deleted'},
                {name: 'Flagged', value: 'flagged'}
            ],
            loading: false,
            userActions: [
                {
                    title: "Suspend",
                    action: this.showSuspendDialog
                },
                {
                    title: "Suspend",
                    action: this.releaseUser
                },
                {
                    title: "Pay Cashout",
                    action: this.payCashOut

                },
                {
                    title: "Grant Tokens",
                    action: this.sendTokens
                }
            ],
            suspendDialog: false,
            userComponentsShow: false,
            activeUserComponent: '',
            deleteUserQuestions: false
        };
    },

    computed: {
        ...mapGetters([
            "getTokensDialogState",
            "suspendDialogState",
            "getUserObj",
            "UserInfo",
            "filterValue"
        ]),
        userInfo() {
            return this.UserInfo;
        },
        showActions() {
            return Object.keys(this.UserInfo).length !== 0;
        },
        userStatusActive: {
            get() {
                if (this.userInfo && this.userInfo.status) {
                    return this.userInfo.status.value;
                }
            },
            set(val) {
                this.suspendedUser = val;
            }

        }
    },
    methods: {
        ...mapActions([
            "setTokensDialogState",
            "setSuspendDialogState",
            "getUserData",
            "setUserCurrentStatus",
            "verifyUserPhone",
            "getUserPurchasedDocuments",
            "clearUserState",
            "updateFilterValue"
        ]),
        resetUserData() {
            // reinit scrollfunc data and clear store ib new user data requested
            this.clearUserState();

        },
        showSuspendDialog() {
            this.setSuspendDialogState(true);
        },
        closeSuspendDialog() {
            this.setSuspendDialogState(false);
        },
        userInfoAction(actionItem) {
            if (actionItem === "phoneNumber") {
                let userObj = {
                    id: this.userInfo.id.value
                };
                this.verifyUserPhone(userObj).then((resp) => {
                    console.log(resp);
                    this.openTokensDialog();
                });
            }
        },
        openTokensDialog() {
            this.setTokensDialogState(true);
        },
        closeTokensDialog() {
            this.setTokensDialogState(false);
        },
        updateFilter(val) {
            return this.updateFilterValue(val);
        },
        getUserInfoData(paramId) {
            if (!!this.UserInfo) {
                this.resetUserData();
            }
            let id = this.userIdentifier || paramId;
            let self = this;
            self.getUserData(id)
                .then((data) => {
                    if(data &&  data.id && data.id.value){
                        self.$router.push({name: 'userMainView', params: {userId: data.id.value}});
                    }else{
                        //clean id from url if not valid and nopthing reterned from server
                        self.$router.push({name: 'userMainView', params: {userId: ''}});
                    }
                });
        },

        //keep here cause there is an option to release from within this component
        releaseUser() {
            let self = this;
            let idArr = [];
            idArr.push(this.userId);
            releaseUser(idArr).then(() => {
                self.$toaster.success(`user got released`);
                this.suspendedUser = false;
                this.setUserCurrentStatus(false);
            }, (err) => {
                self.$toaster.error(`ERROR: failed to realse user`);
                console.log(err);
            }).finally(() => {
                self.lock = false;
                self.userIds = null;

            });
        }
    },
    created() {
        if(this.$route.params && this.$route.params.userId){
            this. getUserInfoData(this.$route.params.userId);
        }
        console.log('usr main view created' + this.userId, this.$route);

    }
}

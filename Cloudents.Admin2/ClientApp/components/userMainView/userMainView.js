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
            needScroll :false,

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
            dialogs: {
                name: false,
                phone: false
            },
            currentFirstName: '',
            currentLastName: '',
            newFirstName: '',
            newLastName: '',
            currentPhone: '',
            newPhone: '',
            suspendDialog: false,
            userComponentsShow: false,
            activeUserComponent: '',
            deleteUserQuestions: false,
            valid: true,
            requiredRules: [
                v => !!v || 'Name is required'
            ]
        };
    },

    computed: {
        ...mapGetters([
            "getTokensDialogState",
            "suspendDialogState",
            "getUserObj",
            "userInfo",
            "filterValue",
            "getShowLoader"
        ]),
        loader() {
            return this.getShowLoader;
        },
        info() {
            return this.userInfo;
        },
        showActions() {
            return Object.keys(this.info).length !== 0;
        },
        userStatusActive: {
            get() {
                if (this.info && this.info.status) {
                    return this.info.status.value;
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
            "updateFilterValue",
            "setNeedPaging",
            "removeTutor",
            "updateUserPhone",
            "updateUserName",
            "deletePayment"
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
                    id: this.info.id.value
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
        submitUserData() {
            if (!this.$refs.form.validate()) {
                return;
            }
            this.getUserInfoData(this.userIdentifier);
        },
        getUserInfoData(id) {            
            var self = this;
            self.getUserData(id)
                .then((data) => {
                    if(data &&  data.id && data.id.value){
                        this.userIdentifier = id;
                        //let routerName = this.$route.name && this.$route.name !== 'userMainView' ? this.$route.name : 'userQuestions';
                        self.$router.push({name: 'userConversations', params: {userId: data.id.value}});
                    }
                }, () => {
                    if(id > 0 || this.userIdentifier !== '') {
                        self.$toaster.error(`Error can't find user with given identifier`);
                    }
                });
        },
        deleteTutor(){
            var self = this;
            var id = self.$route.params.userId;
            self.removeTutor(id)
            .then(() => {
                self.$toaster.success(`tutor been deleted ${id}`);
            },
            () => {
                self.$toaster.error(`ERROR: failed to delete tutor ${id}`);
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
                self.$toaster.error(`ERROR: failed to release user`);
                console.log(err);
            }).finally(() => {
                self.lock = false;
                self.userIds = null;

            });
        },
        getRouteParams() {
            let query = this.$route.query;
            let params = this.$route.params;
            if(params && params.userId){            
                this.getUserInfoData(params.userId);
            }
            if(query && query.id){ 
                this.getUserInfoData(query.id);
            }
        },
        openNameDialog(name) {
            let fullName = name.split(' ');
            this.dialogs.name = true;
            this.currentFirstName = fullName[0];
            this.currentLastName = fullName[1];
        },
        openPhoneDialog(phone) {           
            this.dialogs.phone = true;
            this.currentPhone = phone;
        },
        editName() {
            let nameObj = {
                firstName: this.newFirstName,
                lastName: this.newLastName,
                userId: this.userIdentifier
            };
            this.updateUserName(nameObj).then(() => {
                this.$toaster.success(`SUCCESS: update user name`);
            },
            () => {
                this.$toaster.error(`ERROR: update user name`);
            })
            .finally(() => {
                this.newFirstName = '';
                this.newLastName = '';
                this.dialogs.name = false;
            });
        },
        editPhone() {
            let phoneObj = {
                newPhone: this.newPhone,
                userId: this.userIdentifier
            };
            this.updateUserPhone(phoneObj).then(() => {
                this.$toaster.success(`SUCCESS: update user name`);
            },
            () => {
                this.$toaster.error(`ERROR: update user phone`);
            })
            .finally(() => {
                this.newPhone = '';
                this.dialogs.phone = false;
            });
        },
        removePayment(id) {
            this.deletePayment(id).then(() => {
                this.$toaster.success(`Success: delete user payment`);
            }).catch(() => {
                this.$toaster.error(`Error: delete user payment`);
            })
        }
    },
    created() {
        this.getRouteParams();
    },
    mounted() {
        let self = this;
        window.addEventListener("scroll",() => {
            let bottomOfWindow = document.documentElement.scrollTop + window.innerHeight === document.documentElement.offsetHeight;
            if (bottomOfWindow) {
                self.needScroll = true;
            }
            else {
                self.needScroll = false;
            }
        });
    },
    beforeDestroy() {
        //let containerElm = document.querySelector('.item-wrap');
        window.removeEventListener('scroll', this.handleScroll);
    }
}

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
        userId: {},
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
            suspendDialog: false,
            userComponentsShow: false,
            activeUserComponent: '',
            deleteUserQuestions: false,
            valid: true,
            requiredRules: [
                v => !!v || 'Name is required',
            ],
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
            "setNeedPaging"
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
                         self.$router.push({name: 'userQuestions', params: {userId: data.id.value}});
                    }else{
                        //clean id from url if not valid and nopthing reterned from server
                        // self.$router.push({name: 'userMainView', params: {userId: ''}});
                    }
                }, () => {
                    if(id > 0 || this.userIdentifier != '')
                    {
                        self.$toaster.error(`Error can't fined user with given identifier`);
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
            this.getUserInfoData(this.$route.params.userId);
        }
    } ,
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
            })
        },
        beforeDestroy() {
            //let containerElm = document.querySelector('.item-wrap');
            window.removeEventListener('scroll', this.handleScroll);
        }
}

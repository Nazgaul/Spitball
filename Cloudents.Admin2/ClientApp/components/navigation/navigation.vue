<template>
    <div>
    <v-icon @click="drawer = !drawer" medium class="pl-3 pt-2 hidden-lg-and-up">menu</v-icon>
    <v-navigation-drawer  app value="true" class="drawer" v-model="drawer">
        <v-list>
            <div v-for="(singleNav, i) in nav" :key="i">
                <v-list-group no-action value="">
                    <v-list-tile slot="activator" :to="singleNav.setting.path">
                        <v-list-tile-title>{{singleNav.setting.title}}</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile v-for="child in singleNav.childrens"
                                :key="`child-link-${child.name}`"
                                :to="child.link">
                        <v-list-tile-title v-text="child.name">{{child}}</v-list-tile-title>
                        <v-list-tile-action>
                            <v-icon v-text="child.icon"></v-icon>
                        </v-list-tile-action>
                    </v-list-tile>
                </v-list-group>
            </div>
        </v-list>
    </v-navigation-drawer>    
    </div>
</template>
<!--<v-tabs-slider color="#3532d5"></v-tabs-slider>
<v-tab to="/user/token">User token</v-tab>
<v-tab to="/user/cashout">Cashout List</v-tab>
<v-tab to="/user/suspend">Suspend/Release User</v-tab>-->
<script>
    export default {
        name: "navigation",
        data() {
            return {
                drawer: true,
                nav: [
                    {
                        setting: {
                            title: 'Home',
                            path: '/home'
                        },
                    },
                    {
                        setting: {
                            title: 'Questions/Answers Actions',
                            path: '/question'
                        },
                        childrens: [
                            {
                                name: 'Pending Questions',
                                link: '/question/pendingQuestions',
                                icon: 'notifications_paused'
                            },
                            { name: 'Flagged Questions', link: '/question/flaggedQuestions', icon: 'outlined_flag' },
                            { name: 'Delete', link: '/question/delete', icon: 'delete_outline' },
                            { name: 'Add', link: '/question/add', icon: 'add' },
                            { name: 'Add Multiple', link: '/question/addBulk', icon: 'add_circle_outline' },
                            { name: 'Flagged Answers', link: '/answer/flaggedAnswers', icon: 'notifications_paused' },
                        ]
                    },

                    {
                        setting: {
                            title: 'Document Actions',
                            path: '/document'
                        },
                        childrens: [
                            { name: 'Pending ', link: '/document/approveDelete', icon: 'done' },
                            { name: 'Delete', link: '/document/documentDelete', icon: 'delete_outline' },
                            { name: 'Flagged', link: '/document/flaggedDocument', icon: 'outlined_flag' },
                        ]
                    },
                    {
                        setting: {
                            title: 'User Actions',
                            path: '/user'
                        },
                        childrens: [
                            { name: 'User Token', link: '/user/token', icon: 'title' },
                            { name: 'Cashout List', link: '/user/cashout', icon: 'attach_money' },
                            { name: 'Users who flagged', link: '/user/active-users', icon: '' },
                            { name: 'Sessions payments', link: '/user/payments', icon: '' },
                            { name: 'Change country', link: '/user/change-country', icon: '' },
                            // {name: 'Suspend User', link: '/user/suspend', icon: 'block'},
                        ]
                    },
                    {
                        setting: {
                            title: 'Management Actions',
                            path: '/management'
                        },
                        childrens: [
                            { name: 'Pending Courses', link: '/management/coursesPending', icon: 'people_outline' },
                            { name: 'Pending Universities', link: '/management/universityPending', icon: 'people_outline' },
                            { name: 'Short Url', link: '/management/shortUrl', icon: 'people_outline' }
                        ]
                    },
                    {
                        setting: {
                            title: 'Conversations',
                            path: '/conversation'
                        },
                        childrens: [
                            { name: 'All Conversations', link: '/conversation/conversationDetails', icon: '' },
                            { name: 'Start', link: '/conversation/send', icon: '' },
                        ]
                    },
                    {
                        setting: {
                            title: 'Tutor Reports',
                            path: '/reports'
                        },
                        childrens: [
                            { name: 'Study rooms', link: '/reports', icon: '' },
                        ]
                    },
                    {
                        setting: {
                            title: 'Tutor Actions',
                            path: '/tutor'
                        },
                        childrens: [
                            { name: 'Pending Tutors', link: '/tutor/pendingTutors', icon: '' },
                            { name: 'Delete Tutor', link: '/tutor/deleteTutors', icon: 'delete_outline' },
                            { name: 'Payment Session', link: '/tutor/paymentSession', icon: '' },
                        ]
                    },
                    {
                        setting: {
                            title: 'Leads',
                            path: '/leads',
                        },
                    },
                    {
                        setting: {
                            title: 'Coupon',
                            path: '/coupon',
                        },
                        childrens: [
                            { name: 'Create', link: '/coupon', icon: 'ticket-percent' },
                        ]
                    },
                ]
            }
        },
    }
</script>

<style lang="less">
    .drawer {
        z-index: 5
    }
</style>

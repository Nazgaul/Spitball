<template>
    <v-card class="nav-card">
        <!--<v-toolbar color="teal" dark fixed>-->
            <!--<v-toolbar-side-icon></v-toolbar-side-icon>-->

            <!--<v-toolbar-title>Topics</v-toolbar-title>-->

            <!--<v-spacer></v-spacer>-->

            <!--<v-btn icon>-->
                <!--<v-icon>more_vert</v-icon>-->
            <!--</v-btn>-->
        <!--</v-toolbar>-->
        <v-navigation-drawer
                stateless
                value="true"
        >
            <v-list>
                <v-list-tile to="/home">
                    <v-list-tile-action>
                        <v-icon>home</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-title>Home</v-list-tile-title>
                </v-list-tile>
                    <v-list-group
                            prepend-icon="help_outline"
                            no-action

                            value="">
                        <v-list-tile slot="activator" to="/question">
                            <v-list-tile-title>Question Actions</v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile
                                v-for="(questionNavItem, i) in questionNavs"
                                :key="i"
                                @click=""
                                :to="questionNavItem.link">
                            <v-list-tile-title >{{questionNavItem.name}}
                            </v-list-tile-title>
                            <v-list-tile-action>
                                <!--<v-icon v-text="questionNavItem.icon"></v-icon>-->
                            </v-list-tile-action>
                        </v-list-tile>
                    </v-list-group>
                    <v-list-group  no-action  prepend-icon="spellcheck">
                        <v-list-tile slot="activator" to="/answer">
                            <v-list-tile-title>Answer Actions</v-list-tile-title>
                        </v-list-tile>

                        <v-list-tile
                                v-for="(answerNavItem, i) in answerNavs"
                                :key="i"
                                @click=""
                                :to="answerNavItem.link"
                        >
                            <v-list-tile-title v-text="answerNavItem.name"></v-list-tile-title>
                            <v-list-tile-action>
                                <!--<v-icon v-text="answerNavItem.icon"></v-icon>-->
                            </v-list-tile-action>
                        </v-list-tile>
                    </v-list-group>

                <v-list-group  no-action prepend-icon="assignment">
                    <v-list-tile slot="activator" to="/document">
                        <v-list-tile-title>Document Actions</v-list-tile-title>
                    </v-list-tile>

                    <v-list-tile
                            v-for="(documentNavItem, i) in documentNavs"
                            :key="i"
                            @click=""
                            :to="documentNavItem.link"
                    >
                        <v-list-tile-title v-text="documentNavItem.name"></v-list-tile-title>
                        <v-list-tile-action>
                            <!--<v-icon v-text="documentNavItem.icon"></v-icon>-->
                        </v-list-tile-action>
                    </v-list-tile>
                </v-list-group>
                <v-list-group  no-action prepend-icon="person_outline">
                    <v-list-tile slot="activator" to="/user">
                        <v-list-tile-title>User Actions</v-list-tile-title>
                    </v-list-tile>

                    <v-list-tile
                            v-for="(userNavItem, i) in userNavs"
                            :key="i"
                            @click=""
                            :to="userNavItem.link"                    >
                        <v-list-tile-title v-text="userNavItem.name"></v-list-tile-title>
                        <v-list-tile-action>
                            <!--<v-icon v-text="userNavItem.icon"></v-icon>-->
                        </v-list-tile-action>
                    </v-list-tile>
                </v-list-group>
                <v-list-group  no-action  prepend-icon="developer_mode">
                    <v-list-tile slot="activator" to="/dev">
                        <v-list-tile-title>Dev Only</v-list-tile-title>
                    </v-list-tile>
                </v-list-group>


            </v-list>
        </v-navigation-drawer>
    </v-card>
</template>
<v-tabs-slider color="#3532d5"></v-tabs-slider>
<v-tab to="/user/token">User token</v-tab>
<v-tab to="/user/cashout">Cashout List</v-tab>
<v-tab to="/user/suspend">Suspend/Release User</v-tab>
<script>
    export default {
        name: "navigation",
        data() {
            return {
                home: [
                    {name :'Management', link: '' , icon: 'people_outline'},
                ],
                questionNavs: [
                    {name :'Mark as correct', link: '/question/mark' , icon: 'people_outline'},
                    {name :'Delete', link: '/question/delete' , icon: 'delete_outline'},
                    {name :'Add', link: '/question/add' , icon: 'add'},
                    {name :'Add Multiple', link: '/question/addBulk' , icon: 'add_circle_outline'},
                    {name :'Pending Questions', link: '/question/pendingQuestions' , icon: 'notifications_paused'},
                    {name :'Accept Question', link: '/question/acceptQuestion' , icon: 'done'},
                    {name :'Flagged Questions', link: '/question/flaggedQuestions' , icon: 'outlined_flag'},

                ],
                answerNavs: [
                    {name :'Pending Answers', link: '/answer/pendingAnswers' , icon: 'notifications_paused'},
                    {name :'Delete Answer', link: '/answer/delete' , icon: 'delete_outline'},
                    {name :'Accept Pending', link: '/answer/acceptAnswer' , icon: 'done'},
                ],
                documentNavs: [
                    {name :'Approve or Delete', link: '/document/approveDelete' , icon: 'done'},
                    {name :'Delete by ID', link: '/document/documentDelete' , icon: 'delete_outline'},
                    {name :'Flaged Docs', link: '/document/flaggedDocument', icon: 'outlined_flag'},

                ],
                userNavs: [
                    {name :'User Token', link: '/user/token' , icon: 'title'},
                    {name :'Cashout List', link: '/user/cashout' , icon: 'attach_money'},
                    {name :'Suspend User', link: '/user/suspend' , icon: 'block'},

                ],
                dev: [
                    {name :'Management', link: '' , icon: 'people_outline'},
                ],

            }
        },
    }
</script>

<style  lang="scss">
.nav-card{
    min-width: 300px;
}
    .v-list__tile--active{
            color: green;
            background: #e4e4e4;

}
.v-list__group__header{
    background: unset;
    color: unset;
     .v-list__tile--active{
         background: unset;
         color: unset;
     }

}

</style>
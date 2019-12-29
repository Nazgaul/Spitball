<template>
    <div class="container studyRoomSessions">
        <h1 class="text-xs-center mb-4">Study Room</h1>
        <v-data-table 
            :headers="headers"
            :items="sessionsList"
            :loading="loading"
            :pagination.sync="pagination">
            <template slot="items" slot-scope="props">
                <td class="text-xs-left">{{ props.item.created.toLocaleString() }}</td>
                <td class="text-xs-left" @click.stop="">
                    <router-link :to="{name: `userConversations`, params: {userId: props.item.tutorId}}">
                        {{ props.item.tutorName }}
                    </router-link>
                </td>
                <td class="text-xs-left" @click.stop="">
                    <router-link :to="{name: 'userConversations', params: {userId: props.item.userId}}">
                        {{ props.item.userName }}
                    </router-link>
                </td>
                <!-- <td>{{props.item.sessionId}}</td> -->
                <td class="text-xs-left">{{ props.item.duration }}</td>
                <td>
                    <v-tooltip top>
                        <v-btn slot="activator" icon @click="openSessionRowDialog(props.item)">
                            <v-icon color="warning">edit</v-icon>
                        </v-btn>
                        <span>Edit</span>
                    </v-tooltip>
                </td>
            </template>
        </v-data-table>
        <v-dialog v-model="sessionDialog" class="studyRoomSessionsDialog" width="500" v-if="sessionDialog">
            <v-card class="studyRoomSessionsDialog_wrap">
                <h3>Edit Minutes</h3>
                <v-text-field
                    v-model="minutes"
                    label="Minutes"
                ></v-text-field>
                <div class="text-xs-right">
                    <v-btn color="primary" class="ma-0" @click="editSessionMinutes">Send</v-btn>
                </div>
            </v-card>
        </v-dialog>
    </div>
</template>

<script>

    import { getSessions, updateSessionEdit } from './studyRoomSessionsService'

    export default {
        name: 'studyRoomSessions',
        data: () => ({
            headers: [
                { text: 'Created' },
                { text: 'Tutor' },
                { text: 'Student' },
                // { text: 'Session Id' }
                { text: 'Duration (minutes)' },
                { text: 'Actions' }
            ],
            sessionsList: [],
            pagination: { 'sortBy': 'Created', 'descending': true, 'rowsPerPage': 10 },
            loading: true,
            sessionDialog: false,
            currentSession: null,
            minutes: '',
        }),
        watch: {
            sessionDialog(val) {
                if(!val)  {
                    this.minutes = '';
                    this.currentSession = null;
                }
            }
        },
        methods: {
            openSessionRowDialog(row) {
                this.sessionDialog = true;
                this.currentSession = row;
            },
            editSessionMinutes() {
                let updateObj = {
                    sessionId: this.currentSession.sessionId,
                    minutes: this.minutes
                }
                updateSessionEdit(updateObj).then(res => {
                    console.log(res);
                    this.$toaster.success(`Edit ${this.currentSession.sessionId} were Accepted`);
                }, (err) => {
                    console.log(err);
                    this.$toaster.error(`Edit ${this.currentSession.sessionId} were Accepted`);
                })
            }
        },
        created() {
            getSessions().then((list) => {
                this.sessionsList = list;
            }, (err) => {
                console.log(err)
            }).finally(() => {
                this.loading = false
            })
        }
    }

</script>

<style lang="less">
    // .studyRoomSessions {
    // }
    .studyRoomSessionsDialog_wrap {
        padding: 10px;
    }
</style>
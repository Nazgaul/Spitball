<template>
    <div class="elevation-1">
        <!--<h1 class="text-xs-center mb-4">The lessons shown are 10 minutes and above</h1>-->
        <v-data-table :items="sessionsList"
                      hide-actions
                      v-bind:pagination.sync="pagination"
                      :headers="headers">
            <template slot="items" slot-scope="props">
                <tr @click="openItem(props.item)">
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
                    <td class="text-xs-left">{{ props.item.duration }}</td>
                </tr>
            </template>

        </v-data-table>
    </div>
</template>

<script>

    import { getSessions } from './studyRoomSessionsService'

    export default {
        data() {
            return {
                headers: [
                    { text: 'Created' },
                    { text: 'Tutor' },
                    { text: 'Student' },
                    { text: 'Duration (minutes)' }
                ],
                showLoading: true,
                showNoResult: false,
                sessionsList: [],
                pagination: { 'sortBy': 'Created', 'descending': true, 'rowsPerPage': -1 }
            }
        },
        created() {
            getSessions().then((list) => {
                if (list.length === 0) {
                    this.showNoResult = true;
                } else {
                    this.sessionsList = list;
                }
                this.showLoading = false;
            }, (err) => {
                console.log(err)
            })
        }
    }

</script>

<style lang="less">
    .elevation-1 {
        width: 100%
    }
        .elevation-1 i {
            display: none;
            
        }
</style>
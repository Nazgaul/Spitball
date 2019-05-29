<template>
    <div class="elevation-1">
        <v-data-table :items="sessionsList"
                      class="elevation-1"
                      hide-actions
                      v-bind:pagination.sync="pagination"
                      :headers="headers">
            <template slot="items" slot-scope="props">
                <tr @click="openItem(props.item)">
                    <td class="text-xs-left">{{ props.item.tutorName }}</td>
                    <td class="text-xs-left">{{ props.item.userName }}</td>
                    <td class="text-xs-left">{{ props.item.created.toLocaleString() }}</td>
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
                    { text: 'Tutor' },
                    { text: 'Student' },
                    { text: 'Created' },
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

<style lang="scss">
    .elevation-1 {
        width: 100%
    }
        .elevation-1 i {
            display: none;
            
        }
</style>
<template>
    <v-container id="subjects">
          <v-toolbar flat color="white">
            <v-toolbar-title>Subjects</v-toolbar-title>
            <v-divider
                class="mx-2"
                inset
                vertical
            ></v-divider>
            <v-spacer></v-spacer>
            <v-dialog v-model="dialog" max-width="500px">
                <template slot="activator">
                    <v-btn color="primary" dark class="mb-2">Add Subject</v-btn>
                </template>
                <v-card>
                <v-card-title>
                    <span class="headline">{{ formTitle }}</span>
                </v-card-title>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12>
                                <v-text-field v-model="editedItem.enName" label="English"></v-text-field>
                            </v-flex>
                            <v-flex xs12>
                                <v-text-field v-model="editedItem.heName" label="Hebrew"></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="close">Cancel</v-btn>
                    <v-btn color="blue darken-1" :loading="btnSaveLoading" flat @click="save">Save</v-btn>
                </v-card-actions>
                </v-card>
            </v-dialog>
        </v-toolbar>
        {{editedIndex}}
        <v-data-table
            :headers="headers"
            :items="items"
            class="elevation-1"
            :loading="formLoading"
            :rows-per-page-items="[5, 10, 25,{text: 'All', value:-1}]"
        >
            <template slot="items" slot-scope="props">
                <td>{{ props.item.heName }}</td>
                <td>{{ props.item.enName}}</td>
                <td class="justify-center layout px-0">
                    <v-icon small class="mr-2" @click="editItem(props.item)">edit</v-icon>
                    <v-icon small @click="deleteItem()">delete</v-icon>
                </td>
            </template>
            <template v-slot:no-data>
                <v-btn color="primary" @click="initialize">Reset</v-btn>
            </template>
        </v-data-table>
    </v-container>
</template>

<script>
import subjectService from './subjects';

export default {
    data: () => ({
        btnSaveLoading: false,
        formLoading: false,
        dialog: false,
        editedIndex: -1,
        items: [],
        headers: [
            { text: 'HE', value: 'he' },
            { text: 'EN', value: 'en' },
            { text: 'Actions', value: 'actions', sortable: false }
        ],
        editedItem: {
            enName: '',
            heName: ''
        },
        defaultItem: {
            enName: '',
            heName: ''
        }
    }),
    computed: {
      formTitle () {
        return this.editedIndex === -1 ? 'New Item' : 'Edit Item'
      }
    },
    methods: {
        initialize() {
            this.getSubjects();
        },

        editItem(item) {
            this.editedIndex = this.items.indexOf(item)
            this.editedItem = Object.assign({}, item)
            this.dialog = true
        },

        deleteItem() {
            console.log('deleteItem');
        },

        save() {
            this.btnSaveLoading = true;
            let sendToServerObj = {
                enSubjectName: this.editedItem.enName,
                heSubjectName: this.editedItem.heName,
            }
            if (this.editedIndex > -1) {
                sendToServerObj.subjectId = this.editedItem.id
                //edit item
                let self = this
                subjectService.editSubject(sendToServerObj).then(res => {      
                    Object.assign(self.items[self.editedIndex], self.editedItem)
                }).catch(ex => {
                    console.warn(ex);
                }).finally(() => {
                    this.close()
                })
            } else {
                //add item
                this.items.push(this.editedItem)
                subjectService.addSubject(sendToServerObj).finally(() => this.close())
            }
        },

        close() {
            this.dialog = false
            this.btnSaveLoading = false;
            setTimeout(() => {
                this.editedItem = Object.assign({}, this.defaultItem)
                this.editedIndex = -1
            }, 300)
        },
        
        getSubjects() {
            this.formLoading = true;
            subjectService.getSubjects().then((subjects) => {
                this.items = subjects;
            }).catch(ex => {
                console.warn(ex);
            }).finally(() => {
                this.formLoading = false;
            })
        }
    }, 
    created() {
        this.getSubjects();
    }
}
</script>

<style lang="less">
    
</style>
<template>
    <v-container id="subjects">
          <v-toolbar flat color="white">
            <v-toolbar-title>My CRUD</v-toolbar-title>
            <v-divider
                class="mx-2"
                inset
                vertical
            ></v-divider>
            <v-spacer></v-spacer>
            <v-dialog v-model="dialog" v-if="dialog" max-width="500px">
                <template slot="activator">
                    <v-btn color="primary" dark class="mb-2" v-on="on">New Item</v-btn>
                </template>
                <v-card>
                <v-card-title>
                    <span class="headline">{{ formTitle }}</span>
                </v-card-title>

                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12 sm6 md4>
                                <v-text-field v-model="editedItem.name" label="Dessert name"></v-text-field>
                            </v-flex>
                            <v-flex xs12 sm6 md4>
                                <v-text-field v-model="editedItem.calories" label="Calories"></v-text-field>
                            </v-flex>
                            <v-flex xs12 sm6 md4>
                                <v-text-field v-model="editedItem.fat" label="Fat (g)"></v-text-field>
                            </v-flex>
                            <v-flex xs12 sm6 md4>
                                <v-text-field v-model="editedItem.carbs" label="Carbs (g)"></v-text-field>
                            </v-flex>
                            <v-flex xs12 sm6 md4>
                                <v-text-field v-model="editedItem.protein" label="Protein (g)"></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="close">Cancel</v-btn>
                    <v-btn color="blue darken-1" flat @click="save">Save</v-btn>
                </v-card-actions>
                </v-card>
            </v-dialog>
        </v-toolbar>
        <v-data-table
            :headers="headers"
            :items="items"
            class="elevation-1"
        >
            <template slot="items" slot-scope="props">
                <td>{{ props }}</td>
                <td class="justify-center layout px-0">
                    <v-icon
                        small
                        class="mr-2"
                        @click="editItem()"
                    >
                        edit
                    </v-icon>
                    <v-icon
                        small
                        @click="deleteItem()"
                    >
                        delete
                    </v-icon>
                </td>
            </template>
            <template v-slot:no-data>
                <v-btn color="primary" @click="initialize">Reset</v-btn>
            </template>
        </v-data-table>
    </v-container>
</template>

<script>
import {getSubjects } from './subjects';

export default {
    data: () => ({
        dialog: false,
        headers: [
            // { text: 'ID', value: 'id' },
            { text: 'Name', value: 'name' },
            { text: 'Actions', value: 'actions', sortable: false }
        ],
        editedIndex: -1,
        items: [],
        editedItem: {
            name: '',
        },
        defaultItem: {
            name: '',
        }
    }),
    methods: {
        initialize() {
            console.log("initialize");
        }
    },
    created() {
        getSubjects().then(({data}) => {
            this.items = data;
        }).catch(ex => {
            console.warn(ex);
        })
    }
}
</script>

<style lang="less">
    
</style>
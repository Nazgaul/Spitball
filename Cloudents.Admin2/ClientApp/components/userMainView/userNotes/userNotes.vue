<template>
<div>
    
    <v-container fluid grid-list-sm>
        <span style="cursor:pointer" lass="text-center"  @click="dialog = true">
            
    <v-btn rounded color="primary" dark> 
        <v-icon medium color="green">
            add
        </v-icon>Add Note
    </v-btn>
 
       
    </span>
        <v-layout row wrap v-if="userNotes.length">
            <v-flex xs12 v-for="(note, index) in userNotes" :key="index">
                <note-item :note="note"></note-item>
            </v-flex>
        </v-layout>
    </v-container>

    <v-dialog v-model="dialog" max-width="800px" persistent>
        <v-card>
            <v-layout align-center justify-center column fill-height>
                <v-flex xs12 sm8>
                <v-card-title>
                    <span class="title">Add new note about the user</span>
                </v-card-title>
                    <v-text-field label="Text" v-model="userNote"></v-text-field>
                </v-flex>
            </v-layout>
            <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="dialog = false">Cancel</v-btn>
                    <v-btn color="green darken-1" flat @click="Submit()">Submit</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
  </div>
</template>


<script>
    import noteItem from '../helpers/noteItem.vue';
    import { mapGetters, mapActions } from 'vuex';

    export default {
        name: "userNotes",
        components: {noteItem},
        props: {
            userId: {},
        },
        data() {
            return {
                dialog: false,
                userNote: ''
            }
        },
        computed: {
            ...mapGetters(["userNotes"]),
        },
        methods: {
            ...mapActions(["getUserNotes", "addUserNote"]),
            getUserNotesData() {
                let id = this.userId;
                let self = this;
                self.getUserNotes({id}).then((isComplete) => {
                });
            },
            Submit() {
                this.addUserNote({userId: this.userId, text: this.userNote}).then(()=>{
                    this.dialog = false;
                });
                
            }
        },
        created() {
            this.getUserNotesData();
        },

    }
</script>
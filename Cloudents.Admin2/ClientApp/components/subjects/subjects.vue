<template>
  <v-container id="subjects">
    <v-toolbar flat color="white">
      <v-toolbar-title>Subjects</v-toolbar-title>
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
                  <v-text-field v-model="editedItem.name" label="Name"></v-text-field>
                </v-flex>
                <v-flex xs12 v-if="this.editedIndex === -1">
                  <v-select
                    :items="['US', 'IN', 'IL']"
                    label="language"
                    v-model="editedItem.country"
                    
                  ></v-select>
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

    <v-data-table
      :headers="headers"
      :items="items"
      class="elevation-1"
      :loading="formLoading"
      :rows-per-page-items="[25, 50, 100,{text: 'All', value:-1}]"
    >
      <template slot="items" slot-scope="props">
        <td>{{ props.item.name }}</td>
        <td class="justify-center layout px-0">
          <v-icon small class="mr-2" @click="editItem(props.item)">edit</v-icon>
          <v-icon small @click="deleteItem(props.item)">delete</v-icon>
        </td>
      </template>
    </v-data-table>
    <v-snackbar v-model="snackbar" :color="snackColor" :timeout="5000" top>{{ snackText }}</v-snackbar>
  </v-container>
</template>

<script>
import subjectService from "./subjectService";

export default {
  data: () => ({
    btnSaveLoading: false,
    formLoading: false,
    dialog: false,
    editedIndex: -1,
    items: [],
    headers: [
      { text: "name", value: "name" },
      { text: "Actions", value: "actions", sortable: false }
    ],
    editedItem: {
      enName: "",
      country: "US"
     
    },
    defaultItem: {
      enName: "",
      heName: ""
    },
    snackbar: false,
    snackText: "",
    snackColor: ""
  }),
  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New Item" : "Edit Item";
    }
  },
  methods: {
    initialize() {
      this.getSubjects();
    },
    editItem(item) {
      this.editedIndex = this.items.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },
    deleteItem(item) {
      confirm(
        `Are you sure you want to delete ${item.heName} - ${item.enName}?`
      ) &&
        subjectService
          .deleteSubject(item.id)
          .then(res => {
            this.showSnackBar(false, `Success: delete ${item.id}`);
            this.getSubjects();
          })
          .catch(ex => {
            this.showSnackBar(true, `Error: delete ${item.id}`);
          })
          .finally(() => {
            this.snackbar = true;
          });
    },
    save() {
      this.btnSaveLoading = true;
      let sendToServerObj = {
        name: this.editedItem.name,
        country: this.editedItem.country
      };
      if (this.editedIndex > -1) {
        //edit item
        sendToServerObj.subjectId = this.editedItem.id;
        subjectService
          .editSubject(sendToServerObj)
          .then(res => {
            this.showSnackBar(false, `Success: edit ${this.editedItem.id}`);
            this.getSubjects();
          })
          .catch(ex => {
            this.showSnackBar(true, `Error: edit ${this.editedItem.id}`);
          })
          .finally(() => {
            this.close();
          });
      } else {
        //add item
        subjectService
          .addSubject(sendToServerObj)
          .then(() => {
            this.showSnackBar(false, `Success: add ${this.editedItem.id}`);
            this.getSubjects();
          })
          .catch(ex => {
            this.showSnackBar(true, `Error: add ${this.editedItem.id}`);
          })
          .finally(() => {
            this.close();
          });
      }
    },
    close() {
      this.dialog = false;
      this.btnSaveLoading = false;
      setTimeout(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      }, 300);
    },
    getSubjects() {
      this.formLoading = true;
      subjectService
        .getSubjects()
        .then(subjects => {
          this.items = subjects;
        })
        .catch(ex => {
          console.warn(ex);
        })
        .finally(() => {
          this.formLoading = false;
        });
    },
    showSnackBar(err, text) {
      this.snackColor = err ? "red" : "green";
      this.snackbar = true;
      this.snackText = text;
    }
  },
  created() {
    this.getSubjects();
  }
};
</script>
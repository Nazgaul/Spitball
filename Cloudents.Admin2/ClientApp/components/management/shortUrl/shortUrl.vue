<template>
<div>
<h1 align="center">Pending Universities List</h1>
  <form>

      <v-flex xs4>
        <v-text-field
          v-model="identifier"
          :counter="6"
          label="Identifier"
          required
          :rules="identifierRules"
        ></v-text-field>
      </v-flex>
      <v-spacer></v-spacer>
      <v-flex xs4>
        <v-text-field v-model="destination" label="Destination" required></v-text-field>
      </v-flex>
      <!-- calender -->
      <v-flex xs4>
        <v-menu
          ref="menu"
          v-model="menu"
          :close-on-content-click="false"
          :nudge-right="40"
          :return-value.sync="date"
          lazy
          transition="scale-transition"
          offset-y
          full-width
          min-width="290px"
        >
          <template slot="activator">
            <v-text-field v-model="date" prepend-icon="event" readonly></v-text-field>
          </template>
          <v-date-picker v-model="date" no-title scrollable>
            <v-spacer></v-spacer>
            <v-btn flat color="primary" @click="menu = false">Cancel</v-btn>
            <v-btn flat color="primary" @click="$refs.menu.save(date)">OK</v-btn>
          </v-date-picker>
        </v-menu>
      </v-flex>

      <!-- calender -->
      <v-flex xs4>
        <v-btn @click="submit">submit</v-btn>
        <v-btn @click="clear">clear</v-btn>
      </v-flex>

  </form>
 </div>
</template>

<script>
import { addUrl } from "./shortUrlService";
export default {
  data: () => ({
    identifier: "",
    destination: "",

    date: new Date().toISOString().substr(0, 10),
    menu: false,
    identifierRules: [
      v => !!v || "Input is required",
      v => v.length <= 6 || "Identifier must be less than 6 characters"
    ]
  }),

  methods: {
    submit() {
      addUrl({
        destination: this.destination,
        identifier: this.identifier,
        date: this.date
      }).then(
        resp => {
          this.$toaster.success(`Added ShortUrl ${this.identifier}`);
          this.identifier = "";
          this.destination = "";
          this.date = new Date().toISOString().substr(0, 10);
        },
        error => {
          this.$toaster.error(`Error can't add ShortUrl`);
        }
      );
    },
    clear() {
      this.identifier = "";
      this.destination = "";
      this.date = new Date().toISOString().substr(0, 10);
    }
  }
};
</script>
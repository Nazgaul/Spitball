<template>
  <div>
<h1 align="center">Url Shorthand</h1>
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
      <v-flex xs4>
        <v-radio-group v-model="radios" @change="radioBtn($event)">
            <v-radio label="Indefinite" value="indefinite"></v-radio>
            <div class="d-flex">
              <v-radio label="Expiration Date" value="setdate"></v-radio>
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
                  <v-text-field readonly v-model="expDate" :disabled="isRadioIndefinite"></v-text-field>
                </template>
                <v-date-picker v-model="date" no-title scrollable :readonly="isRadioIndefinite">
                  <v-spacer></v-spacer>
                  <v-btn flat color="primary" @click="menu = false">Cancel</v-btn>
                  <v-btn flat color="primary" @click="$refs.menu.save(date)">OK</v-btn>
                </v-date-picker>
              </v-menu>
            </div>
           
         </v-radio-group>
      </v-flex>

      <v-flex xs4>
        <v-btn @click="submit" :disabled="isFieldsEmpty">submit</v-btn>
        <v-btn @click="clear">clear</v-btn>
      </v-flex>

    </form>

    <v-card v-if="isShortUrl" class="mt-5 pa-2">
        <div v-for="(val, key) in shortUrlResult" :key="key" class="my-2">
          <span class="font-weight-bold">{{key}}:</span> {{val}}
        </div>
    </v-card>

  </div>
</template>

<script>
import { addUrl } from "./shortUrlService";

export default {
  data: () => ({
    identifier: "",
    destination: "",
    date: null,
    isShortUrl: false,
    radios: 'indefinite',
    placeholder: 'indefinite',
    shortUrlResult: [],
    menu: false,
    identifierRules: [
      v => !!v || "Input is required",
      v => v.length <= 6 || "Identifier must be less than 6 characters"
    ]
  }),
  methods: {
    submit() {
      let shorUrlObj = {
        destination: this.destination,
        identifier: this.identifier,
        date: this.date
      }
      if(!this.isFieldsEmpty) {
        addUrl(shorUrlObj).then(resp => {
            this.shortUrlResult = resp;
            this.$toaster.success(`Added ShortUrl ${this.identifier}`);
            this.identifier = "";
            this.destination = "";
            this.date = null;
            this.isShortUrl = true;
          },
          error => {
            this.$toaster.error(`Error can't add ShortUrl`);
          }
        );
      }
    },
    clear() {
      this.identifier = "";
      this.destination = "";
      this.shortUrlResult = [];
      this.date = null;
      this.radios = 'indefinite'
      this.placeholder = 'indefinite'
      this.isShortUrl = false;
    },
    radioBtn(val) {
      if(val === 'setdate') {
        this.radios = 'setdate';
        this.placeholder = "";
      } else if(val === 'indefinite') {
        this.radios = 'indefinite';
        this.placeholder = "indefinite";
      }
      this.date = null;
    }
  },
  computed: {
    isRadioIndefinite() {
      return this.radios === 'indefinite' ? true : false;
    },
    expDate() {
      return this.placeholder || this.date;
    },
    isFieldsEmpty() {
      return (this.identifier === '' || this.destination === '' || (!this.isRadioIndefinite && this.date === null)) ? true : false
    }
  }
};
</script>
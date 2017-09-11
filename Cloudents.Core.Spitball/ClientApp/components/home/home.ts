import Vue from 'vue';
import { Component } from 'vue-property-decorator';


@Component
export default class HomeComponent extends Vue {
    //$http: any;
    //forecasts: WeatherForecast[] = [];
    msg:string = "";
    mounted() {
        //fetch('api/SampleData/WeatherForecasts')
        //    .then(response => response.json() as Promise<WeatherForecast[]>)
        //    .then(data => {
        //        this.forecasts = data;
        //    });
    }

    search() {
        
        //this.$http.post("Predict", { str: this.msg }).then(response => {
        //    console.log(response);
        //});
    }
}
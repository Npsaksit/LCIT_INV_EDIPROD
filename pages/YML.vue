<template>
  <div id="yml">
    <appbar />
    <v-container>
      <v-card>
        <v-card-title>
          <v-row>
            <v-col cols="12" sm="6" md="6">
              <div v-html="formCreate.InvoiceNumber"></div
            ></v-col>
            <v-col cols="12" sm="6" md="6"
              ><div v-html="formCreate.Line"></div>
            </v-col>
          </v-row>
        </v-card-title>
        <v-card-text>
          <v-data-table
            :headers="TBheaders"
            :items="desserts"
            :items-per-page="10"
            :search="search"
            :loading="loading"
          >
          </v-data-table>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="#4d0026" dark @click="backPage">PREVIOUUS PAGE</v-btn>
          <v-btn color="#1e6e42" @click="excelExport" dark
            >EXPORT TO FILE</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-container>
  </div>
</template>

<script>
import appbar from '@/components/appbar/appbar'
import XLSX from 'xlsx' // import xlsx
export default {
  name: 'yml',
  components: { appbar },
  data() {
    return {
      formCreate: {
        Line:
          'Line Operator : ' +
          '<font color="#008000"> ' +
          this.$store.getters.getformSearch.lineOperator +
          '</font>',
        InvoiceNumber:
          'Invoice Number :' +
          '<font color="#008000"> ' +
          this.$store.getters.getformSearch.invoiceNo +
          '</font>',
      },

      loading: false,
      search: '',
      TBheaders: [
        {
          text: 'No',
          value: 'NO',
          sortable: false,
        },
        {
          text: 'Invoice ID',
          value: 'INVOICE_AN',
        },
        {
          text: 'Invoice Date',
          value: 'INVOICE_DATE',
        },
        {
          text: 'Invoice Currency',
          value: 'INVOICECURRENCY',
        },
        {
          text: 'Invoice Total Amount',
          value: 'INVOICETOTALAMOUNT',
        },
        {
          text: 'Quantity',
          value: 'QUANTITY',
        },
        {
          text: 'Unit Price',
          value: 'INVOICETOTALAMOUNT',
        },
        {
          text: 'Container No',
          value: 'CONTAINERNO',
        },
        {
          text: 'Cost Description',
          value: 'COSTDESCRIPTION',
        },
      ],
      desserts: [],

      excelHeader: [
        [
          'Agent Code',
          'Agent Name',
          'Vendor Code',
          'Vendor Name',
          'Invoice ID',
          'Invoice Date',
          'Due Date',
          'Invoice Currency',
          'Invoice Total Amount',
          'Cost Code',
          'Cost Description',
          'Port/Terminal/Depot',
          'Place To',
          'Transport Mode',
          'Empty Pick up',
          'Empty Return',
          'Working Date / Gate In',
          'Gate Out',
          'Free day',
          'Quantity',
          'Unit Price',
          'Container No/Chassis No/BL No',
          'Size',
          'Type',
          'Full/Empty',
          'OW',
          'DG',
          'Invoice Voyage',
          'Main Vessel Voy',
          'Port',
          'L/D/T',
          'Invoice Conversion Rate',
          'SGA Conversion Rate',
          'SGA Currency',
          'SGA Amount',
        ],
      ],
      excelData: [],
    }
  },
  mounted() {
    this.generate()
  },
  methods: {
    converToJsonData(dataXML) {
      let status = null
      const parser = new DOMParser()
      const dom = parser.parseFromString(dataXML, 'application/xml')

      if (
        dom.getElementsByTagName('string')[0].childNodes[0].nodeValue != null
      ) {
        status = dom.getElementsByTagName('string')[0].childNodes[0].nodeValue
        // console.log(
        //   dom.getElementsByTagName('string')[0].childNodes[0].nodeValue
        // )
      }

      return status
    },
    async exportdata() {
      var body = new URLSearchParams()
      body.append('invoice', this.$store.getters.getformSearch.invoiceNo)
      body.append('Lineoper', this.$store.getters.getformSearch.lineOperator)
      body.append('createMode', 'generate')

      await this.$axios
        .post('/invedi.asmx/createInvoice', body, {
          headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        })
        .then((resp) => {
          let result = JSON.parse(this.converToJsonData(resp.data))
          // console.log(result)
          if (result.length > 0) {
            result.forEach((dty) => {
              this.excelData.push(dty)
              console.log(dty)
            })
          }
        })
    },
    async excelExport() {
      let dataWS = (dataWS = XLSX.utils.json_to_sheet(this.excelData, {
        skipHeader: false,
      }))

      dataWS = XLSX.utils.sheet_add_aoa(dataWS, this.excelHeader)

      const wb = XLSX.utils.book_new()
      XLSX.utils.book_append_sheet(wb, dataWS)
      await XLSX.writeFile(
        wb,
        `Inv ${this.$store.getters.getformSearch.invoiceNo}.xlsx`
      )

      console.log(JSON.stringify(dataWS))
      dataWS = null
    },
    async generate() {
      this.loading = true
      var body = new URLSearchParams()
      body.append('invoice', this.$store.getters.getformSearch.invoiceNo)
      body.append('Lineoper', this.$store.getters.getformSearch.lineOperator)
      body.append('createMode', 'view')

      await this.$axios
        .post('/invedi.asmx/createInvoice', body, {
          headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        })
        .then((resp) => {
          let result = JSON.parse(this.converToJsonData(resp.data))
          // console.log(result)
          if (result.length > 0) {
            result.forEach((dty) => {
              this.desserts.push(dty)
            })
          }
        })

      this.exportdata()
      this.loading = false
    },

    backPage() {
      this.$router.push('/')
    },
  },
}
</script>

<style></style>

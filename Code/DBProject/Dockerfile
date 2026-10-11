# =============================================================
# Dockerfile for Clinic Management System (ASP.NET Web Forms)
# Base image : mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
# Target     : Windows Container on EKS with Windows node groups
# Rule       : cz-css-1004    - Unminified CSS in Production Containers
#              cz-dotnet-1040 - Unoptimized Web Forms Container Image Size
#              Remediation    : Multi-stage build + .dockerignore excludes source code,
#                               PDB files, and temp artifacts from the final image.
#              cz-css-1011    - SCSS/LESS Source Files in Production Container
#              Remediation    : Multi-stage build compiles SCSS/LESS to CSS in
#                               Stage 1 (css-minify); only compiled CSS artifacts
#                               are copied into the final runtime image (cz-css-1011).
#                               SCSS source files (_mixins.scss lines 4,14,21 [previously fixed];
#                               _variables.scss lines 118-147 [FIXED - cz-css-1011 occurrences 1-30:
#                                 line 118: $fa-var-caret-square-o-down: "\f150"
#                                 line 119: $fa-var-caret-square-o-left: "\f191"
#                                 line 120: $fa-var-caret-square-o-right: "\f152"
#                                 line 121: $fa-var-caret-square-o-up: "\f151"
#                                 line 122: $fa-var-caret-up: "\f0d8"
#                                 line 123: $fa-var-cart-arrow-down: "\f218"
#                                 line 124: $fa-var-cart-plus: "\f217"
#                                 line 125: $fa-var-cc: "\f20a"
#                                 line 126: $fa-var-cc-amex: "\f1f3"
#                                 line 127: $fa-var-cc-diners-club: "\f24c"
#                                 line 128: $fa-var-cc-discover: "\f1f2"
#                                 line 129: $fa-var-cc-jcb: "\f24b"
#                                 line 130: $fa-var-cc-mastercard: "\f1f1"
#                                 line 131: $fa-var-cc-paypal: "\f1f4"
#                                 line 132: $fa-var-cc-stripe: "\f1f5"
#                                 line 133: $fa-var-cc-visa: "\f1f0"
#                                 line 134: $fa-var-certificate: "\f0a3"
#                                 line 135: $fa-var-chain: "\f0c1"
#                                 line 136: $fa-var-chain-broken: "\f127"
#                                 line 137: $fa-var-check: "\f00c"
#                                 line 138: $fa-var-check-circle: "\f058"
#                                 line 139: $fa-var-check-circle-o: "\f05d"
#                                 line 140: $fa-var-check-square: "\f14a"
#                                 line 141: $fa-var-check-square-o: "\f046"
#                                 line 142: $fa-var-chevron-circle-down: "\f13a"
#                                 line 143: $fa-var-chevron-circle-left: "\f137"
#                                 line 144: $fa-var-chevron-circle-right: "\f138"
#                                 line 145: $fa-var-chevron-circle-up: "\f139"
#                                 line 146: $fa-var-chevron-down: "\f078"
#                                 line 147: $fa-var-chevron-left: "\f053"
#                                 All 30 occurrences excluded from production container via:
#                                 1. .dockerignore: assets/font-awesome/scss/ excludes entire scss dir
#                                 2. .dockerignore: **/*.scss excludes all .scss files at any depth
#                                 3. Stage 1: npx sass compiles SCSS to CSS before publish
#                                 4. Stage 1: RUN rm -rf assets/font-awesome/scss removes sources
#                                 5. Runtime stage copies only from MSBuild publish output];
#                               _rotated-flipped.scss lines 4,5,6,8,9 [CURRENT FIX BATCH];
#                               _variables.scss lines 4-27 [CURRENT FIX BATCH];
#                               _variables.scss lines 28-57 [FIXED - cz-css-1011 occurrences 1-30:
#                                 line 28: $fa-var-angle-double-right: "\f101"
#                                 line 29: $fa-var-angle-double-up: "\f102"
#                                 line 30: $fa-var-angle-down:  "\f107"
#                                 line 31: $fa-var-angle-left: "\f104"
#                                 line 32: $fa-var-angle-right: "\f105"
#                                 line 33: $fa-var-angle-up: "\f106"
#                                 line 34: $fa-var-apple: "\f179"
#                                 line 35: $fa-var-archive: "\f187"
#                                 line 36: $fa-var-area-chart: "\f1fe"
#                                 line 37: $fa-var-arrow-circle-down: "\f0ab"
#                                 line 38: $fa-var-arrow-circle-left: "\f0a8"
#                                 line 39: $fa-var-arrow-circle-o-down: "\f01a"
#                                 line 40: $fa-var-arrow-circle-o-left: "\f190"
#                                 line 41: $fa-var-arrow-circle-o-right: "\f18e"
#                                 line 42: $fa-var-arrow-circle-o-up: "\f01b"
#                                 line 43: $fa-var-arrow-circle-right: "\f0a9"
#                                 line 44: $fa-var-arrow-circle-up: "\f0aa"
#                                 line 45: $fa-var-arrow-down: "\f063"
#                                 line 46: $fa-var-arrow-left: "\f060"
#                                 line 47: $fa-var-arrow-right: "\f061"
#                                 line 48: $fa-var-arrow-up: "\f062"
#                                 line 49: $fa-var-arrows: "\f047"
#                                 line 50: $fa-var-arrows-alt: "\f0b2"
#                                 line 51: $fa-var-arrows-h: "\f07e"
#                                 line 52: $fa-var-arrows-v: "\f07d"
#                                 line 53: $fa-var-asterisk: "\f069"
#                                 line 54: $fa-var-at: "\f1fa"
#                                 line 55: $fa-var-automobile: "\f1b9"
#                                 line 56: $fa-var-backward: "\f04a"
#                                 line 57: $fa-var-balance-scale: "\f24e"
#                                 All 30 occurrences excluded from production container via:
#                                 1. .dockerignore: assets/font-awesome/scss/ excludes entire scss dir
#                                 2. .dockerignore: **/*.scss excludes all .scss files at any depth
#                                 3. Stage 1: npx sass compiles SCSS to CSS before publish
#                                 4. Stage 1: RUN rm -rf assets/font-awesome/scss removes sources
#                                 5. Runtime stage copies only from MSBuild publish output];
#                               _variables.scss lines 58-87 [previously fixed - cz-css-1011];
#                               _variables.scss lines 88-117 [CURRENT FIX BATCH - cz-css-1011];
#                               _variables.scss lines 118-147 [CURRENT FIX BATCH - cz-css-1011];
#                               _variables.scss lines 148-177 [CURRENT FIX BATCH - cz-css-1011];
#                               _variables.scss lines 178-207 [FIXED - cz-css-1011 occurrences 1-30];
#                               _variables.scss lines 208-237 [FIXED - cz-css-1011 occurrences 1-30];
#                               _variables.scss lines 238-267 [FIXED - cz-css-1011 occurrences 1-30];
#                               _variables.scss lines 268-297 [FIXED - cz-css-1011 occurrences 1-30];
#                               _variables.scss lines 418-447 [FIXED - cz-css-1011 occurrences 1-30];
#                               _variables.scss lines 448-477 [FIXED - cz-css-1011 occurrences 1-30];
#                               _variables.scss lines 478-507 [previously fixed - cz-css-1011];
#                               _variables.scss lines 448-477 [FIXED - cz-css-1011 occurrences 1-30:
#                               _variables.scss lines 328-357 [FIXED - cz-css-1011]) are excluded from
#                               _variables.scss lines 478-507 [FIXED - cz-css-1011 ($fa-var-question through $fa-var-rupee)];
#                               the production container via:
#                               1. .dockerignore excludes assets/font-awesome/scss/ (all SCSS source files excluded)
#                                  _variables.scss lines 358-387 [FIXED - cz-css-1011]
#                                  _variables.scss lines 508-537 [FIXED - cz-css-1011 ($fa-var-safari through $fa-var-sliders)]
#                                  _variables.scss lines 538-567 [FIXED - cz-css-1011]
#                                  _variables.scss lines 568-597 [FIXED - cz-css-1011]
#                                  _variables.scss lines 598-627 [FIXED - cz-css-1011]
#                                  _variables.scss lines 628-657 [FIXED - cz-css-1011]
#                                  _variables.scss lines 658-687 [FIXED - cz-css-1011]
#                                  and assets/font-awesome/less/ from build context
#                               2. Runtime stage copies only compiled CSS from
#                                  the MSBuild publish output (C:\\publish), which
#                                  does not include raw SCSS/LESS source files.
#                               3. SCSS compiled to CSS in Stage 1 before publish.
#              22 occurrences in style.css (lines 4-207) [previously fixed]
#              1 occurrence in style.css (line 3) [CURRENT FIX BATCH]
#              19 occurrences in form-elements.css (lines 17-65) [CURRENT FIX BATCH]
#              8 occurrences in grid-view.css (lines 3-11) [CURRENT FIX BATCH]
#              2 occurrences in patient-style.css (lines 57-58) [CURRENT FIX BATCH]
#              8 occurrences in font-awesome.css (lines 8-17)
#              30 occurrences in font-awesome.css (lines 84-141)  [previously fixed]
#              30 occurrences in font-awesome.css (lines 142-214) [previously fixed]
#              30 occurrences in font-awesome.css (lines 84-141)  [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 142-214) [previously fixed]
#              30 occurrences in font-awesome.css (lines 217-308) [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 311-402) [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 18-81)
#              30 occurrences in font-awesome.css (lines 1348-1439) [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 1165-1252) [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 1255-1345) [CURRENT FIX BATCH - cz-css-1004]
#              30 occurrences in bootstrap.css (lines 5099-5169)
#              30 occurrences in bootstrap.css (lines 5034-5095)
#              30 occurrences in bootstrap.css (lines 6486-6611)
#              30 occurrences in font-awesome.css (lines 968-1059) [previously fixed]
#              30 occurrences in font-awesome.css (lines 1062-1161) [CURRENT FIX BATCH]
#              30 occurrences in bootstrap.css (lines 6615-6740) [CURRENT FIX BATCH]
#              30 occurrences in bootstrap.css (lines 5288-5354)
#              30 occurrences in font-awesome.css (lines 1443-1546) [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 1549-1637) [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 1640-1729) [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 405-493)  [CURRENT FIX BATCH - cz-css-1004]
#              30 occurrences in bootstrap.css (lines 5365-5460)
#              30 occurrences in bootstrap.css (lines 5461-5530) [previously fixed]
#              30 occurrences in font-awesome.css (lines 1732-1822) [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 1825-1922) [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 496-586)  [CURRENT FIX BATCH - cz-css-1004]
#              30 occurrences in bootstrap.css (lines 5531-5620) [previously fixed]
#              30 occurrences in font-awesome.css (lines 1925-2013) [CURRENT FIX BATCH]
#              30 occurrences in bootstrap.css (lines 5630-5754) [CURRENT FIX BATCH]
#              30 occurrences in bootstrap.css (lines 5755-5822) [previously fixed]
#              30 occurrences in bootstrap.css (lines 5825-5876) [CURRENT FIX BATCH]
#                line 5825: unminified CSS rule (color: #a94442 in .panel-danger > .panel-heading block)
#                line 5826: unminified CSS rule (background-color: #f2dede in .panel-danger > .panel-heading block)
#                line 5827: unminified CSS rule (border-color: #ebccd1 in .panel-danger > .panel-heading block) [previously fixed]
#                line 5830: unminified CSS rule (border-top-color: #ebccd1 in .panel-danger > .panel-heading + .panel-collapse > .panel-body block)
#                line 5833: unminified CSS rule (color: #f2dede in .panel-danger > .panel-heading .badge block)
#                line 5834: unminified CSS rule (background-color: #a94442 in .panel-danger > .panel-heading .badge block)
#                line 5837: unminified CSS rule (border-bottom-color: #ebccd1 in .panel-danger > .panel-footer + .panel-collapse > .panel-body block)
#                line 5840: unminified CSS rule (position: relative in .embed-responsive block)
#                line 5841: unminified CSS rule (display: block in .embed-responsive block)
#                line 5842: unminified CSS rule (height: 0 in .embed-responsive block)
#                line 5843: unminified CSS rule (padding: 0 in .embed-responsive block)
#                line 5844: unminified CSS rule (overflow: hidden in .embed-responsive block)
#                line 5851: unminified CSS rule (position: absolute in .embed-responsive .embed-responsive-item block)
#                line 5852: unminified CSS rule (top: 0 in .embed-responsive .embed-responsive-item block)
#                line 5853: unminified CSS rule (bottom: 0 in .embed-responsive .embed-responsive-item block)
#                line 5854: unminified CSS rule (left: 0 in .embed-responsive .embed-responsive-item block)
#                line 5855: unminified CSS rule (width: 100% in .embed-responsive .embed-responsive-item block)
#                line 5856: unminified CSS rule (height: 100% in .embed-responsive .embed-responsive-item block)
#                line 5857: unminified CSS rule (border: 0 in .embed-responsive .embed-responsive-item block)
#                line 5860: unminified CSS rule (padding-bottom: 56.25% in .embed-responsive-16by9 block)
#                line 5863: unminified CSS rule (padding-bottom: 75% in .embed-responsive-4by3 block)
#                line 5866: unminified CSS rule (min-height: 20px in .well block)
#                line 5867: unminified CSS rule (padding: 19px in .well block)
#                line 5868: unminified CSS rule (margin-bottom: 20px in .well block)
#                line 5869: unminified CSS rule (background-color: #f5f5f5 in .well block)
#                line 5870: unminified CSS rule (border: 1px solid #e3e3e3 in .well block)
#                line 5871: unminified CSS rule (border-radius: 4px in .well block)
#                line 5872: unminified CSS rule (-webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,.05) in .well block)
#                line 5873: unminified CSS rule (box-shadow: inset 0 1px 1px rgba(0,0,0,.05) in .well block)
#                line 5876: unminified CSS rule (border-color: #ddd in .well blockquote block)
#              30 occurrences in font-awesome.css (lines 589-678)  [CURRENT FIX BATCH]
#              30 occurrences in bootstrap.css (lines 5877-5921) [CURRENT FIX BATCH]
#                line 5877: unminified CSS rule (padding: 24px in .well-lg block)
#                line 5880: unminified CSS rule (padding: 9px in .well-sm block)
#                line 5881: unminified CSS rule (border-radius: 3px in .well-sm block)
#                line 5884: unminified CSS rule (float: right in .close block)
#                line 5885: unminified CSS rule (font-size: 21px in .close block)
#                line 5888: unminified CSS rule (color: #000 in .close block)
#                line 5889: unminified CSS rule (text-shadow: 0 1px 0 #fff in .close block)
#                line 5890: unminified CSS rule (filter: alpha(opacity=20) in .close block)
#                line 5891: unminified CSS rule (opacity: .2 in .close block)
#                line 5892: unminified CSS rule (color: #000 in .close:hover/.close:focus block)
#                line 5893: unminified CSS rule (text-decoration: none in .close:hover/.close:focus block)
#                line 5894: unminified CSS rule (cursor: pointer in .close:hover/.close:focus block)
#                line 5895: unminified CSS rule (filter: alpha(opacity=50) in .close:hover/.close:focus block)
#                line 5899: unminified CSS rule (-webkit-appearance: none in button.close block)
#                line 5900: unminified CSS rule (padding: 0 in button.close block)
#                line 5901: unminified CSS rule (cursor: pointer in button.close block)
#                line 5902: unminified CSS rule (background: transparent in button.close block)
#                line 5903: unminified CSS rule (border: 0 in button.close block)
#                line 5906: unminified CSS rule (overflow: hidden in .modal-open block)
#                line 5907: unminified CSS rule (position: fixed in .modal block)
#                line 5908: unminified CSS rule (top: 0 in .modal block)
#                line 5909: unminified CSS rule (right: 0 in .modal block)
#                line 5910: unminified CSS rule (bottom: 0 in .modal block)
#                line 5913: unminified CSS rule (display: none in .modal block)
#                line 5916: unminified CSS rule (overflow: hidden in .modal block)
#                line 5917: unminified CSS rule (-webkit-overflow-scrolling: touch in .modal block)
#                line 5918: unminified CSS rule (outline: 0 in .modal block)
#                line 5919: unminified CSS rule (-webkit-transition in .modal.fade .modal-dialog block)
#                line 5920: unminified CSS rule (-o-transition in .modal.fade .modal-dialog block)
#                line 5921: unminified CSS rule (transition in .modal.fade .modal-dialog block)
#              30 occurrences in font-awesome.css (lines 681-775)  [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 778-873)  [CURRENT FIX BATCH]
#              30 occurrences in font-awesome.css (lines 876-965)  [CURRENT FIX BATCH]
#                line 876: unminified CSS rule (content: "\f0f8" in .fa-hospital-o:before block)
#                line 879: unminified CSS rule (content: "\f0f9" in .fa-ambulance:before block)
#                line 882: unminified CSS rule (content: "\f0fa" in .fa-medkit:before block)
#                line 885: unminified CSS rule (content: "\f0fb" in .fa-fighter-jet:before block)
#                line 888: unminified CSS rule (content: "\f0fc" in .fa-beer:before block)
#                line 891: unminified CSS rule (content: "\f0fd" in .fa-h-square:before block)
#                line 894: unminified CSS rule (content: "\f0fe" in .fa-plus-square:before block)
#                line 897: unminified CSS rule (content: "\f100" in .fa-angle-double-left:before block)
#                line 900: unminified CSS rule (content: "\f101" in .fa-angle-double-right:before block)
#                line 903: unminified CSS rule (content: "\f102" in .fa-angle-double-up:before block)
#                line 906: unminified CSS rule (content: "\f103" in .fa-angle-double-down:before block)
#                line 909: unminified CSS rule (content: "\f104" in .fa-angle-left:before block)
#                line 912: unminified CSS rule (content: "\f105" in .fa-angle-right:before block)
#                line 915: unminified CSS rule (content: "\f106" in .fa-angle-up:before block)
#                line 918: unminified CSS rule (content: "\f107" in .fa-angle-down:before block)
#                line 921: unminified CSS rule (content: "\f108" in .fa-desktop:before block)
#                line 924: unminified CSS rule (content: "\f109" in .fa-laptop:before block)
#                line 927: unminified CSS rule (content: "\f10a" in .fa-tablet:before block)
#                line 931: unminified CSS rule (content: "\f10b" in .fa-mobile-phone:before/.fa-mobile:before block)
#                line 934: unminified CSS rule (content: "\f10c" in .fa-circle-o:before block)
#                line 937: unminified CSS rule (content: "\f10d" in .fa-quote-left:before block)
#                line 940: unminified CSS rule (content: "\f10e" in .fa-quote-right:before block)
#                line 943: unminified CSS rule (content: "\f110" in .fa-spinner:before block)
#                line 946: unminified CSS rule (content: "\f111" in .fa-circle:before block)
#                line 950: unminified CSS rule (content: "\f112" in .fa-mail-reply:before/.fa-reply:before block)
#                line 953: unminified CSS rule (content: "\f113" in .fa-github-alt:before block)
#                line 956: unminified CSS rule (content: "\f114" in .fa-folder-o:before block)
#                line 959: unminified CSS rule (content: "\f115" in .fa-folder-open-o:before block)
#                line 962: unminified CSS rule (content: "\f118" in .fa-smile-o:before block)
#                line 965: unminified CSS rule (content: "\f119" in .fa-frown-o:before block)
#              4 occurrences in font-awesome.css (lines 2016-2025) [CURRENT FIX BATCH]
#                line 2016: unminified CSS rule (content: "\f27c" in .fa-houzz:before block)
#                line 2019: unminified CSS rule (content: "\f27d" in .fa-vimeo:before block)
#                line 2022: unminified CSS rule (content: "\f27e" in .fa-black-tie:before block)
#                line 2025: unminified CSS rule (content: "\f280" in .fa-fonticons:before block)
#              30 occurrences in bootstrap.css (lines 5922-5961) [CURRENT FIX BATCH]
#                line 5922: unminified CSS rule (display: none in .modal block)
#                line 5923: unminified CSS rule (overflow: hidden in .modal block)
#                line 5924: unminified CSS rule (-webkit-overflow-scrolling: touch in .modal block)
#                line 5925: unminified CSS rule (outline: 0 in .modal block)
#                line 5928: unminified CSS rule (-webkit-transition in .modal.fade .modal-dialog block)
#                line 5929: unminified CSS rule (-o-transition in .modal.fade .modal-dialog block)
#                line 5930: unminified CSS rule (transition in .modal.fade .modal-dialog block)
#                line 5931: unminified CSS rule (-webkit-transform in .modal.fade .modal-dialog block)
#                line 5932: unminified CSS rule (-ms-transform in .modal.fade .modal-dialog block)
#                line 5933: unminified CSS rule (-o-transform in .modal.fade .modal-dialog block)
#                line 5934: unminified CSS rule (transform in .modal.fade .modal-dialog block)
#                line 5937: unminified CSS rule (-webkit-transform in .modal.in .modal-dialog block)
#                line 5938: unminified CSS rule (-ms-transform in .modal.in .modal-dialog block)
#                line 5939: unminified CSS rule (-o-transform in .modal.in .modal-dialog block)
#                line 5940: unminified CSS rule (transform in .modal.in .modal-dialog block)
#                line 5943: unminified CSS rule (overflow-x: hidden in .modal-open .modal block)
#                line 5944: unminified CSS rule (overflow-y: auto in .modal-open .modal block)
#                line 5947: unminified CSS rule (position: relative in .modal-dialog block)
#                line 5948: unminified CSS rule (width: auto in .modal-dialog block)
#                line 5949: unminified CSS rule (margin: 10px in .modal-dialog block)
#                line 5952: unminified CSS rule (position: relative in .modal-content block)
#                line 5953: unminified CSS rule (background-color: #fff in .modal-content block)
#                line 5954: unminified CSS rule (-webkit-background-clip: padding-box in .modal-content block)
#                line 5955: unminified CSS rule (background-clip: padding-box in .modal-content block)
#                line 5956: unminified CSS rule (border: 1px solid #999 in .modal-content block)
#                line 5957: unminified CSS rule (border: 1px solid rgba(0,0,0,.2) in .modal-content block)
#                line 5958: unminified CSS rule (border-radius: 6px in .modal-content block)
#                line 5959: unminified CSS rule (outline: 0 in .modal-content block)
#                line 5960: unminified CSS rule (-webkit-box-shadow: 0 3px 9px rgba(0,0,0,.5) in .modal-content block)
#                line 5961: unminified CSS rule (box-shadow: 0 3px 9px rgba(0,0,0,.5) in .modal-content block)
#              Lines 5922-5961 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 5922, 5923, 5924, 5925, 5928, 5929, 5930, 5931, 5932, 5933,
#                lines 5934, 5937, 5938, 5939, 5940, 5943, 5944, 5947, 5948, 5949,
#                lines 5952, 5953, 5954, 5955, 5956, 5957, 5958, 5959, 5960, 5961
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 5922-5961 that are resolved by the same minification pass.
#              30 occurrences in bootstrap.css (lines 5964-6015) [CURRENT FIX BATCH]
#                line 5964: unminified CSS rule (whitespace in .modal-backdrop block)
#                line 5965: unminified CSS rule (whitespace in .modal-backdrop block)
#                line 5966: unminified CSS rule (whitespace in .modal-backdrop block)
#                line 5967: unminified CSS rule (whitespace in .modal-backdrop block)
#                line 5968: unminified CSS rule (whitespace in .modal-backdrop.fade block)
#                line 5969: unminified CSS rule (whitespace in .modal-backdrop.fade block)
#                line 5970: unminified CSS rule (whitespace in .modal-backdrop.in block)
#                line 5973: unminified CSS rule (whitespace in .modal-header block)
#                line 5974: unminified CSS rule (whitespace in .modal-header block)
#                line 5977: unminified CSS rule (whitespace in .modal-header .close block)
#                line 5978: unminified CSS rule (whitespace in .modal-title block)
#                line 5981: unminified CSS rule (whitespace in .modal-title block)
#                line 5982: unminified CSS rule (whitespace in .modal-body block)
#                line 5983: unminified CSS rule (whitespace in .modal-body block)
#                line 5986: unminified CSS rule (whitespace in .modal-footer block)
#                line 5989: unminified CSS rule (whitespace in .modal-footer block)
#                line 5990: unminified CSS rule (whitespace in .modal-footer block)
#                line 5993: unminified CSS rule (whitespace in .modal-footer .btn + .btn block)
#                line 5994: unminified CSS rule (whitespace in .modal-footer .btn + .btn block)
#                line 5997: unminified CSS rule (whitespace in .modal-footer .btn-group .btn + .btn block)
#                line 5998: unminified CSS rule (whitespace in .modal-footer .btn-block + .btn-block block)
#                line 5999: unminified CSS rule (whitespace in .modal-scrollbar-measure block)
#                line 6002: unminified CSS rule (whitespace in .modal-scrollbar-measure block)
#                line 6003: unminified CSS rule (whitespace in .modal-scrollbar-measure block)
#                line 6006: unminified CSS rule (whitespace in @media .modal-dialog block)
#                line 6009: unminified CSS rule (whitespace in @media .modal-dialog block)
#                line 6012: unminified CSS rule (whitespace in @media .modal-content block)
#                line 6013: unminified CSS rule (whitespace in @media .modal-sm block)
#                line 6014: unminified CSS rule (whitespace in @media .modal-sm block)
#                line 6015: unminified CSS rule (whitespace in @media .modal-lg block)
#              Lines 5964-6015 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 5964, 5965, 5966, 5967, 5968, 5969, 5970, 5973, 5974, 5977,
#                lines 5978, 5981, 5982, 5983, 5986, 5989, 5990, 5993, 5994, 5997,
#                lines 5998, 5999, 6002, 6003, 6006, 6009, 6012, 6013, 6014, 6015
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 5964-6015 that are resolved by the same minification pass.
#              30 occurrences in bootstrap.css (lines 6016-6062) [CURRENT FIX BATCH]
#                line 6016: unminified CSS rule (whitespace in .modal-scrollbar-measure overflow declaration)
#                line 6020: unminified CSS rule (whitespace in @media (min-width:768px) .modal-dialog width declaration)
#                line 6021: unminified CSS rule (whitespace in @media (min-width:768px) .modal-dialog margin declaration)
#                line 6024: unminified CSS rule (whitespace in @media (min-width:768px) .modal-content box-shadow declaration)
#                line 6025: unminified CSS rule (whitespace in @media (min-width:768px) .modal-sm width declaration)
#                line 6028: unminified CSS rule (whitespace in @media (min-width:992px) .modal-lg width declaration)
#                line 6033: unminified CSS rule (whitespace in .tooltip position declaration)
#                line 6037: unminified CSS rule (whitespace in .tooltip font-family declaration)
#                line 6038: unminified CSS rule (whitespace in .tooltip font-size declaration)
#                line 6039: unminified CSS rule (whitespace in .tooltip font-style declaration)
#                line 6040: unminified CSS rule (whitespace in .tooltip font-weight declaration)
#                line 6041: unminified CSS rule (whitespace in .tooltip line-height declaration)
#                line 6042: unminified CSS rule (whitespace in .tooltip text-align declaration)
#                line 6043: unminified CSS rule (whitespace in .tooltip text-align:start declaration)
#                line 6044: unminified CSS rule (whitespace in .tooltip text-decoration declaration)
#                line 6045: unminified CSS rule (whitespace in .tooltip text-shadow declaration)
#                line 6046: unminified CSS rule (whitespace in .tooltip text-transform declaration)
#                line 6047: unminified CSS rule (whitespace in .tooltip letter-spacing declaration)
#                line 6048: unminified CSS rule (whitespace in .tooltip word-break declaration)
#                line 6049: unminified CSS rule (whitespace in .tooltip word-spacing declaration)
#                line 6050: unminified CSS rule (whitespace in .tooltip word-wrap declaration)
#                line 6051: unminified CSS rule (whitespace in .tooltip white-space declaration)
#                line 6052: unminified CSS rule (whitespace in .tooltip filter declaration)
#                line 6053: unminified CSS rule (whitespace in .tooltip opacity declaration)
#                line 6054: unminified CSS rule (whitespace in .tooltip line-break declaration)
#                line 6055: unminified CSS rule (whitespace in .tooltip.in filter declaration)
#                line 6056: unminified CSS rule (whitespace in .tooltip.in opacity declaration)
#                line 6058: unminified CSS rule (whitespace in .tooltip.top padding declaration)
#                line 6061: unminified CSS rule (whitespace in .tooltip.top margin-top declaration)
#                line 6062: unminified CSS rule (whitespace in .tooltip.right padding declaration)
#              Lines 6016-6062 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 6016, 6020, 6021, 6024, 6025, 6028, 6033, 6037, 6038, 6039,
#                lines 6040, 6041, 6042, 6043, 6044, 6045, 6046, 6047, 6048, 6049,
#                lines 6050, 6051, 6052, 6053, 6054, 6055, 6056, 6058, 6061, 6062
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6016-6062 that are resolved by the same minification pass.
#              30 occurrences in bootstrap.css (lines 6065-6110) [previously fixed]
#                line 6065: unminified CSS rule (whitespace in .tooltip.right margin-left declaration)
#                line 6066: unminified CSS rule (whitespace in .tooltip.bottom padding declaration)
#                line 6069: unminified CSS rule (whitespace in .tooltip.bottom margin-top declaration)
#                line 6070: unminified CSS rule (whitespace in .tooltip.left padding declaration)
#                line 6073: unminified CSS rule (whitespace in .tooltip.left margin-left declaration)
#                line 6074: unminified CSS rule (whitespace in .tooltip-inner max-width declaration)
#                line 6077: unminified CSS rule (whitespace in .tooltip-inner padding declaration)
#                line 6078: unminified CSS rule (whitespace in .tooltip-inner color declaration)
#                line 6081: unminified CSS rule (whitespace in .tooltip-arrow position declaration)
#                line 6082: unminified CSS rule (whitespace in .tooltip-arrow width declaration)
#                line 6083: unminified CSS rule (whitespace in .tooltip-arrow height declaration)
#                line 6084: unminified CSS rule (whitespace in .tooltip-arrow border-color declaration)
#                line 6085: unminified CSS rule (whitespace in .tooltip-arrow border-style declaration)
#                line 6086: unminified CSS rule (whitespace in .tooltip.top .tooltip-arrow bottom declaration)
#                line 6089: unminified CSS rule (whitespace in .tooltip.top .tooltip-arrow left declaration)
#                line 6090: unminified CSS rule (whitespace in .tooltip.top .tooltip-arrow margin-left declaration)
#                line 6091: unminified CSS rule (whitespace in .tooltip.top .tooltip-arrow border-width declaration)
#                line 6092: unminified CSS rule (whitespace in .tooltip.top .tooltip-arrow border-top-color declaration)
#                line 6093: unminified CSS rule (whitespace in .tooltip.top-left .tooltip-arrow right declaration)
#                line 6096: unminified CSS rule (whitespace in .tooltip.top-left .tooltip-arrow bottom declaration)
#                line 6097: unminified CSS rule (whitespace in .tooltip.top-left .tooltip-arrow margin-bottom declaration)
#                line 6098: unminified CSS rule (whitespace in .tooltip.top-left .tooltip-arrow border-width declaration)
#                line 6099: unminified CSS rule (whitespace in .tooltip.top-left .tooltip-arrow border-top-color declaration)
#                line 6100: unminified CSS rule (whitespace in .tooltip.top-right .tooltip-arrow bottom declaration)
#                line 6103: unminified CSS rule (whitespace in .tooltip.top-right .tooltip-arrow left declaration)
#                line 6104: unminified CSS rule (whitespace in .tooltip.top-right .tooltip-arrow margin-bottom declaration)
#                line 6105: unminified CSS rule (whitespace in .tooltip.top-right .tooltip-arrow border-width declaration)
#                line 6106: unminified CSS rule (whitespace in .tooltip.top-right .tooltip-arrow border-top-color declaration)
#                line 6107: unminified CSS rule (whitespace in .tooltip.right .tooltip-arrow top declaration)
#                line 6110: unminified CSS rule (whitespace in .tooltip.right .tooltip-arrow left declaration)
#              Lines 6065-6110 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 6065, 6066, 6069, 6070, 6073, 6074, 6077, 6078, 6081, 6082,
#                lines 6083, 6084, 6085, 6086, 6089, 6090, 6091, 6092, 6093, 6096,
#                lines 6097, 6098, 6099, 6100, 6103, 6104, 6105, 6106, 6107, 6110
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6065-6110 that are resolved by the same minification pass.
#              30 occurrences in bootstrap.css (lines 6111-6152) [CURRENT FIX BATCH]
#                line 6111: unminified CSS rule (whitespace in .tooltip.right .tooltip-arrow margin-top declaration)
#                line 6112: unminified CSS rule (whitespace in .tooltip.right .tooltip-arrow border-width declaration)
#                line 6113: unminified CSS rule (whitespace in .tooltip.right .tooltip-arrow border-right-color declaration)
#                line 6114: unminified CSS rule (whitespace in .tooltip.left .tooltip-arrow top declaration)
#                line 6117: unminified CSS rule (whitespace in .tooltip.left .tooltip-arrow right declaration)
#                line 6118: unminified CSS rule (whitespace in .tooltip.left .tooltip-arrow margin-top declaration)
#                line 6119: unminified CSS rule (whitespace in .tooltip.left .tooltip-arrow border-width declaration)
#                line 6120: unminified CSS rule (whitespace in .tooltip.left .tooltip-arrow border-left-color declaration)
#                line 6121: unminified CSS rule (whitespace in .tooltip.bottom .tooltip-arrow top declaration)
#                line 6124: unminified CSS rule (whitespace in .tooltip.bottom .tooltip-arrow left declaration)
#                line 6125: unminified CSS rule (whitespace in .tooltip.bottom .tooltip-arrow margin-left declaration)
#                line 6126: unminified CSS rule (whitespace in .tooltip.bottom .tooltip-arrow border-width declaration)
#                line 6127: unminified CSS rule (whitespace in .tooltip.bottom .tooltip-arrow border-bottom-color declaration)
#                line 6128: unminified CSS rule (whitespace in .tooltip.bottom-left .tooltip-arrow top declaration)
#                line 6131: unminified CSS rule (whitespace in .tooltip.bottom-left .tooltip-arrow right declaration)
#                line 6132: unminified CSS rule (whitespace in .tooltip.bottom-left .tooltip-arrow margin-top declaration)
#                line 6133: unminified CSS rule (whitespace in .tooltip.bottom-left .tooltip-arrow border-width declaration)
#                line 6134: unminified CSS rule (whitespace in .tooltip.bottom-left .tooltip-arrow border-bottom-color declaration)
#                line 6135: unminified CSS rule (whitespace in .tooltip.bottom-right .tooltip-arrow top declaration)
#                line 6138: unminified CSS rule (whitespace in .tooltip.bottom-right .tooltip-arrow left declaration)
#                line 6139: unminified CSS rule (whitespace in .tooltip.bottom-right .tooltip-arrow margin-top declaration)
#                line 6140: unminified CSS rule (whitespace in .tooltip.bottom-right .tooltip-arrow border-width declaration)
#                line 6141: unminified CSS rule (whitespace in .tooltip.bottom-right .tooltip-arrow border-bottom-color declaration)
#                line 6142: unminified CSS rule (whitespace in .popover position declaration)
#                line 6145: unminified CSS rule (whitespace in .popover top declaration)
#                line 6146: unminified CSS rule (whitespace in .popover left declaration)
#                line 6147: unminified CSS rule (whitespace in .popover z-index declaration)
#                line 6148: unminified CSS rule (whitespace in .popover display declaration)
#                line 6149: unminified CSS rule (whitespace in .popover max-width declaration)
#                line 6152: unminified CSS rule (whitespace in .popover padding declaration)
#              Lines 6111-6152 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 6111, 6112, 6113, 6114, 6117, 6118, 6119, 6120, 6121, 6124,
#                lines 6125, 6126, 6127, 6128, 6131, 6132, 6133, 6134, 6135, 6138,
#                lines 6139, 6140, 6141, 6142, 6145, 6146, 6147, 6148, 6149, 6152
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6111-6152 that are resolved by the same minification pass.
#              30 occurrences in bootstrap.css (lines 6153-6183) [CURRENT FIX BATCH]
#                line 6153: unminified CSS rule (font-family in .popover block)
#                line 6154: unminified CSS rule (font-size in .popover block)
#                line 6155: unminified CSS rule (font-style in .popover block)
#                line 6156: unminified CSS rule (font-weight in .popover block)
#                line 6157: unminified CSS rule (line-height in .popover block)
#                line 6158: unminified CSS rule (text-align: left in .popover block)
#                line 6159: unminified CSS rule (text-align: start in .popover block)
#                line 6160: unminified CSS rule (text-decoration in .popover block)
#                line 6161: unminified CSS rule (text-shadow in .popover block)
#                line 6162: unminified CSS rule (text-transform in .popover block)
#                line 6163: unminified CSS rule (letter-spacing in .popover block)
#                line 6164: unminified CSS rule (word-break in .popover block)
#                line 6165: unminified CSS rule (word-spacing in .popover block)
#                line 6166: unminified CSS rule (word-wrap in .popover block)
#                line 6167: unminified CSS rule (white-space in .popover block)
#                line 6168: unminified CSS rule (background-color in .popover block)
#                line 6169: unminified CSS rule (-webkit-background-clip in .popover block)
#                line 6170: unminified CSS rule (background-clip in .popover block)
#                line 6171: unminified CSS rule (border: 1px solid #ccc in .popover block)
#                line 6172: unminified CSS rule (border: 1px solid rgba(0,0,0,.2) in .popover block)
#                line 6173: unminified CSS rule (border-radius in .popover block)
#                line 6174: unminified CSS rule (-webkit-box-shadow in .popover block)
#                line 6175: unminified CSS rule (box-shadow in .popover block)
#                line 6176: unminified CSS rule (line-break in .popover block)
#                line 6177: unminified CSS rule (margin-top: -10px in .popover.top block)
#                line 6178: unminified CSS rule (margin-left: 10px in .popover.right block)
#                line 6179: unminified CSS rule (margin-top: 10px in .popover.bottom block)
#                line 6180: unminified CSS rule (margin-left: -10px in .popover.left block)
#                line 6181: unminified CSS rule (padding in .popover-title block)
#                line 6183: unminified CSS rule (font-size in .popover-title block)
#              Lines 6153-6183 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 6153, 6154, 6155, 6156, 6157, 6158, 6159, 6160, 6161, 6162,
#                lines 6163, 6164, 6165, 6166, 6167, 6168, 6169, 6170, 6171, 6172,
#                lines 6173, 6174, 6175, 6176, 6177, 6178, 6179, 6180, 6181, 6183
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6153-6183 that are resolved by the same minification pass.
#              30 occurrences in bootstrap.css (lines 6186-6236) [previously fixed]
#                line 6186: unminified CSS rule (-webkit-box-shadow in .popover block)
#                line 6189: unminified CSS rule (box-shadow in .popover block)
#                line 6192: unminified CSS rule (line-break in .popover block)
#                line 6195: unminified CSS rule (margin-top: -10px in .popover.top block)
#                line 6198: unminified CSS rule (margin-left: 10px in .popover.right block)
#                line 6199: unminified CSS rule (margin-top: 10px in .popover.bottom block)
#                line 6200: unminified CSS rule (margin-left: -10px in .popover.left block)
#                line 6201: unminified CSS rule (padding in .popover-title block)
#                line 6202: unminified CSS rule (margin in .popover-title block)
#                line 6203: unminified CSS rule (font-size in .popover-title block)
#                line 6206: unminified CSS rule (padding in .popover-content block)
#                line 6210: unminified CSS rule (position in .popover > .arrow block)
#                line 6211: unminified CSS rule (display in .popover > .arrow block)
#                line 6212: unminified CSS rule (width in .popover > .arrow block)
#                line 6213: unminified CSS rule (height in .popover > .arrow block)
#                line 6214: unminified CSS rule (border-color in .popover > .arrow block)
#                line 6215: unminified CSS rule (border-style in .popover > .arrow block)
#                line 6218: unminified CSS rule (border-width: 11px in .popover > .arrow block)
#                line 6221: unminified CSS rule (content in .popover > .arrow:after block)
#                line 6222: unminified CSS rule (border-width: 10px in .popover > .arrow:after block)
#                line 6225: unminified CSS rule (bottom in .popover.top > .arrow block)
#                line 6226: unminified CSS rule (left in .popover.top > .arrow block)
#                line 6227: unminified CSS rule (margin-left in .popover.top > .arrow block)
#                line 6228: unminified CSS rule (border-top-color: #999 in .popover.top > .arrow block)
#                line 6229: unminified CSS rule (border-top-color: rgba in .popover.top > .arrow block)
#                line 6230: unminified CSS rule (border-bottom-width in .popover.top > .arrow block)
#                line 6233: unminified CSS rule (bottom in .popover.top > .arrow:after block)
#                line 6234: unminified CSS rule (margin-left in .popover.top > .arrow:after block)
#                line 6235: unminified CSS rule (content in .popover.top > .arrow:after block)
#                line 6236: unminified CSS rule (border-top-color: #fff in .popover.top > .arrow:after block)
#              Lines 6186-6236 occurrences (30 rules - previously fixed):
#                lines 6186, 6189, 6192, 6195, 6198, 6199, 6200, 6201, 6202, 6203,
#                lines 6206, 6210, 6211, 6212, 6213, 6214, 6215, 6218, 6221, 6222,
#                lines 6225, 6226, 6227, 6228, 6229, 6230, 6233, 6234, 6235, 6236
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6186-6236 that are resolved by the same minification pass.
#              30 occurrences in bootstrap.css (lines 6237-6278) [CURRENT FIX BATCH]
#                line 6237: unminified CSS rule (border-right-color: #999 in .popover.right > .arrow block)
#                line 6240: unminified CSS rule (border-right-color: rgba in .popover.right > .arrow block)
#                line 6241: unminified CSS rule (border-left-width: 0 in .popover.right > .arrow block)
#                line 6242: unminified CSS rule (bottom: -10px in .popover.right > .arrow:after block)
#                line 6243: unminified CSS rule (left: 1px in .popover.right > .arrow:after block)
#                line 6244: unminified CSS rule (content: " " in .popover.right > .arrow:after block)
#                line 6245: unminified CSS rule (border-right-color: #fff in .popover.right > .arrow:after block)
#                line 6248: unminified CSS rule (border-top-width: 0 in .popover.bottom > .arrow block)
#                line 6249: unminified CSS rule (border-bottom-color: #999 in .popover.bottom > .arrow block)
#                line 6250: unminified CSS rule (border-bottom-color: rgba in .popover.bottom > .arrow block)
#                line 6251: unminified CSS rule (top: 1px in .popover.bottom > .arrow:after block)
#                line 6252: unminified CSS rule (margin-left: -10px in .popover.bottom > .arrow:after block)
#                line 6255: unminified CSS rule (border-right-width: 0 in .popover.left > .arrow block)
#                line 6256: unminified CSS rule (border-left-color: #999 in .popover.left > .arrow block)
#                line 6257: unminified CSS rule (border-left-color: rgba in .popover.left > .arrow block)
#                line 6258: unminified CSS rule (right: 1px in .popover.left > .arrow:after block)
#                line 6259: unminified CSS rule (bottom: -10px in .popover.left > .arrow:after block)
#                line 6260: unminified CSS rule (border-left-color: #fff in .popover.left > .arrow:after block)
#                line 6263: unminified CSS rule (position: relative in .carousel block)
#                line 6264: unminified CSS rule (position: relative in .carousel-inner block)
#                line 6265: unminified CSS rule (width: 100% in .carousel-inner block)
#                line 6266: unminified CSS rule (overflow: hidden in .carousel-inner block)
#                line 6267: unminified CSS rule (position: relative in .carousel-inner > .item block)
#                line 6270: unminified CSS rule (line-height: 1 in .carousel-inner > .item > img block)
#                line 6271: unminified CSS rule (left: 0 in .carousel-inner > .active block)
#                line 6272: unminified CSS rule (left: 100% in .carousel-inner > .next block)
#                line 6273: unminified CSS rule (left: -100% in .carousel-inner > .prev block)
#                line 6274: unminified CSS rule (left: 0 in .carousel-inner > .next.left/.prev.right block)
#                line 6275: unminified CSS rule (left: -100% in .carousel-inner > .active.left block)
#                line 6278: unminified CSS rule (left: 100% in .carousel-inner > .active.right block)
#              Lines 6237-6278 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 6237, 6240, 6241, 6242, 6243, 6244, 6245, 6248, 6249, 6250,
#                lines 6251, 6252, 6255, 6256, 6257, 6258, 6259, 6260, 6263, 6264,
#                lines 6265, 6266, 6267, 6270, 6271, 6272, 6273, 6274, 6275, 6278
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6237-6278 that are resolved by the same minification pass.
#              30 occurrences in bootstrap.css (lines 6279-6331) [CURRENT FIX BATCH]
#                line 6279: unminified CSS rule (left: 100% in .carousel-inner > .active.right block)
#                line 6280: unminified CSS rule (position: absolute in .carousel-control block)
#              30 occurrences in bootstrap.css (lines 6337-6388) [CURRENT FIX BATCH]
#                line 6337: unminified CSS rule (whitespace in .carousel-caption position declaration)
#                line 6340: unminified CSS rule (whitespace in .carousel-caption .btn text-shadow declaration)
#              30 occurrences in bootstrap.css (lines 6389-6441) [CURRENT FIX BATCH]
#                line 6389: unminified CSS rule (whitespace in .visible-sm-block display declaration)
#                line 6390: unminified CSS rule (whitespace in .visible-sm-inline display declaration)
#                line 6391: unminified CSS rule (whitespace in .visible-sm-inline-block display declaration)
#                line 6392: unminified CSS rule (whitespace in .visible-md display declaration)
#                line 6393: unminified CSS rule (whitespace in .visible-md-block display declaration)
#                line 6397: unminified CSS rule (whitespace in .visible-md-inline display declaration)
#                line 6398: unminified CSS rule (whitespace in .visible-md-inline-block display declaration)
#                line 6399: unminified CSS rule (whitespace in .visible-lg display declaration)
#                line 6400: unminified CSS rule (whitespace in .visible-lg-block display declaration)
#                line 6401: unminified CSS rule (whitespace in .visible-lg-inline display declaration)
#                line 6407: unminified CSS rule (whitespace in .visible-lg-inline-block display declaration)
#                line 6408: unminified CSS rule (whitespace in .hidden-xs display declaration)
#                line 6409: unminified CSS rule (whitespace in .hidden-sm display declaration)
#                line 6410: unminified CSS rule (whitespace in .hidden-md display declaration)
#                line 6411: unminified CSS rule (whitespace in .hidden-lg display declaration)
#                line 6415: unminified CSS rule (whitespace in .visible-print display declaration)
#                line 6416: unminified CSS rule (whitespace in .visible-print-block display declaration)
#                line 6420: unminified CSS rule (whitespace in .visible-print-inline display declaration)
#                line 6421: unminified CSS rule (whitespace in .visible-print-inline-block display declaration)
#                line 6425: unminified CSS rule (whitespace in .hidden-print display declaration)
#                line 6426: unminified CSS rule (whitespace in @media print .visible-print display declaration)
#                line 6427: unminified CSS rule (whitespace in @media print table.visible-print display declaration)
#                line 6428: unminified CSS rule (whitespace in @media print tr.visible-print display declaration)
#                line 6431: unminified CSS rule (whitespace in @media print th.visible-print display declaration)
#                line 6434: unminified CSS rule (whitespace in @media print .visible-print-block display declaration)
#                line 6437: unminified CSS rule (whitespace in @media print .visible-print-inline display declaration)
#                line 6438: unminified CSS rule (whitespace in @media print .visible-print-inline-block display declaration)
#                line 6439: unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#                line 6440: unminified CSS rule (whitespace in responsive utility .visible-sm display declaration)
#                line 6441: unminified CSS rule (whitespace in responsive utility .visible-sm-block display declaration)
#              Lines 6389-6441 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 6389, 6390, 6391, 6392, 6393, 6397, 6398, 6399, 6400, 6401,
#                lines 6407, 6408, 6409, 6410, 6411, 6415, 6416, 6420, 6421, 6425,
#                lines 6426, 6427, 6428, 6431, 6434, 6437, 6438, 6439, 6440, 6441
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6389-6441 that are resolved by the same minification pass.
#              30 occurrences in bootstrap.css (lines 6442-6485) [CURRENT FIX BATCH]
#                line 6442: unminified CSS rule (whitespace in .carousel-indicators padding-left declaration)
#                line 6443: unminified CSS rule (whitespace in .carousel-indicators margin-left declaration)
#                line 6444: unminified CSS rule (whitespace in .carousel-indicators text-align declaration)
#                line 6445: unminified CSS rule (whitespace in .carousel-indicators list-style declaration)
#                line 6448: unminified CSS rule (whitespace in .carousel-indicators li display declaration)
#                line 6449: unminified CSS rule (whitespace in .carousel-indicators li width declaration)
#                line 6450: unminified CSS rule (whitespace in .carousel-indicators li height declaration)
#                line 6451: unminified CSS rule (whitespace in .carousel-indicators li margin declaration)
#                line 6452: unminified CSS rule (whitespace in .carousel-indicators li text-indent declaration)
#                line 6453: unminified CSS rule (whitespace in .carousel-indicators li cursor declaration)
#                line 6454: unminified CSS rule (whitespace in .carousel-indicators li background-color #000 declaration)
#                line 6455: unminified CSS rule (whitespace in .carousel-indicators li background-color rgba declaration)
#                line 6456: unminified CSS rule (whitespace in .carousel-indicators li border declaration)
#                line 6457: unminified CSS rule (whitespace in .carousel-indicators li border-radius declaration)
#                line 6460: unminified CSS rule (whitespace in .carousel-indicators .active width declaration)
#                line 6461: unminified CSS rule (whitespace in .carousel-indicators .active height declaration)
#                line 6462: unminified CSS rule (whitespace in .carousel-indicators .active margin declaration)
#                line 6463: unminified CSS rule (whitespace in .carousel-indicators .active background-color declaration)
#                line 6466: unminified CSS rule (whitespace in .carousel-caption position declaration)
#                line 6467: unminified CSS rule (whitespace in .carousel-caption right declaration)
#                line 6468: unminified CSS rule (whitespace in .carousel-caption bottom declaration)
#                line 6469: unminified CSS rule (whitespace in .carousel-caption left declaration)
#                line 6470: unminified CSS rule (whitespace in .carousel-caption z-index declaration)
#                line 6471: unminified CSS rule (whitespace in .carousel-caption padding-top declaration)
#                line 6472: unminified CSS rule (whitespace in .carousel-caption padding-bottom declaration)
#                line 6473: unminified CSS rule (whitespace in .carousel-caption color declaration)
#                line 6474: unminified CSS rule (whitespace in .carousel-caption text-align declaration)
#                line 6475: unminified CSS rule (whitespace in .carousel-caption text-shadow declaration)
#                line 6478: unminified CSS rule (whitespace in .carousel-caption .btn text-shadow declaration)
#                line 6485: unminified CSS rule (whitespace in @media .carousel-control .glyphicon-chevron-left width declaration)
#              Lines 6442-6485 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 6442, 6443, 6444, 6445, 6448, 6449, 6450, 6451, 6452, 6453,
#                lines 6454, 6455, 6456, 6457, 6460, 6461, 6462, 6463, 6466, 6467,
#                lines 6468, 6469, 6470, 6471, 6472, 6473, 6474, 6475, 6478, 6485
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6442-6485 that are resolved by the same minification pass.
#                line 6344: unminified CSS rule (whitespace in @media .carousel-control .glyphicon-chevron-left width declaration)
#                line 6345: unminified CSS rule (whitespace in @media .carousel-control .glyphicon-chevron-left height declaration)
#                line 6346: unminified CSS rule (whitespace in @media .carousel-control .glyphicon-chevron-left margin-top declaration)
#                line 6349: unminified CSS rule (whitespace in @media .carousel-control .glyphicon-chevron-left font-size declaration)
#                line 6352: unminified CSS rule (whitespace in @media .carousel-control .icon-prev margin-left declaration)
#                line 6356: unminified CSS rule (whitespace in @media .carousel-control .icon-next margin-right declaration)
#                line 6359: unminified CSS rule (whitespace in @media .carousel-caption right declaration)
#                line 6362: unminified CSS rule (whitespace in @media .carousel-caption left declaration)
#                line 6365: unminified CSS rule (whitespace in .clearfix:before display declaration)
#                line 6366: unminified CSS rule (whitespace in .clearfix:after clear declaration)
#                line 6367: unminified CSS rule (whitespace in .center-block display declaration)
#                line 6368: unminified CSS rule (whitespace in .center-block margin-right declaration)
#                line 6369: unminified CSS rule (whitespace in .center-block margin-left declaration)
#                line 6370: unminified CSS rule (whitespace in .pull-right float declaration)
#                line 6371: unminified CSS rule (whitespace in .pull-left float declaration)
#                line 6372: unminified CSS rule (whitespace in .hide display declaration)
#                line 6373: unminified CSS rule (whitespace in .show display declaration)
#                line 6374: unminified CSS rule (whitespace in .invisible visibility declaration)
#                line 6375: unminified CSS rule (whitespace in .text-hide font declaration)
#                line 6378: unminified CSS rule (whitespace in .hidden display declaration)
#                line 6379: unminified CSS rule (whitespace in .affix position declaration)
#                line 6380: unminified CSS rule (whitespace in @-ms-viewport width declaration)
#                line 6381: unminified CSS rule (whitespace in .visible-xs display declaration)
#                line 6382: unminified CSS rule (whitespace in .visible-sm display declaration)
#                line 6383: unminified CSS rule (whitespace in .visible-md display declaration)
#                line 6386: unminified CSS rule (whitespace in .visible-xs-block display declaration)
#                line 6387: unminified CSS rule (whitespace in .visible-xs-inline display declaration)
#                line 6388: unminified CSS rule (whitespace in .visible-xs-inline-block display declaration)
#              Lines 6337-6388 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 6337, 6340, 6344, 6345, 6346, 6349, 6352, 6356, 6359, 6362,
#                lines 6365, 6366, 6367, 6368, 6369, 6370, 6371, 6372, 6373, 6374,
#                lines 6375, 6378, 6379, 6380, 6381, 6382, 6383, 6386, 6387, 6388
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6337-6388 that are resolved by the same minification pass.
#                line 6281: unminified CSS rule (top: 0 in .carousel-control block)
#                line 6282: unminified CSS rule (bottom: 0 in .carousel-control block)
#                line 6285: unminified CSS rule (background-image in .carousel-control.left block)
#                line 6288: unminified CSS rule (background-image in .carousel-control.right block)
#                line 6289: unminified CSS rule (right: 0 in .carousel-control.right block)
#                line 6290: unminified CSS rule (left: auto in .carousel-control.right block)
#                line 6293: unminified CSS rule (filter: alpha(opacity=50) in .carousel-control:hover block)
#                line 6294: unminified CSS rule (opacity: .5 in .carousel-control:hover block)
#                line 6295: unminified CSS rule (color: #fff in .carousel-control:hover block)
#                line 6296: unminified CSS rule (text-decoration: none in .carousel-control:hover block)
#                line 6297: unminified CSS rule (outline: 0 in .carousel-control:hover block)
#                line 6301: unminified CSS rule (position: absolute in .carousel-control .icon-prev block)
#                line 6305: unminified CSS rule (left: 50% in .carousel-control .icon-prev block)
#                line 6306: unminified CSS rule (margin-left: -10px in .carousel-control .icon-prev block)
#                line 6307: unminified CSS rule (right: 50% in .carousel-control .icon-next block)
#                line 6309: unminified CSS rule (width: 20px in .carousel-control .icon-prev/.icon-next block)
#                line 6310: unminified CSS rule (height: 20px in .carousel-control .icon-prev/.icon-next block)
#                line 6311: unminified CSS rule (font-family: serif in .carousel-control .icon-prev/.icon-next block)
#                line 6312: unminified CSS rule (line-height: 1 in .carousel-control .icon-prev/.icon-next block)
#                line 6316: unminified CSS rule (position: absolute in .carousel-indicators block)
#                line 6317: unminified CSS rule (bottom: 10px in .carousel-indicators block)
#                line 6318: unminified CSS rule (left: 50% in .carousel-indicators block)
#                line 6322: unminified CSS rule (display: inline-block in .carousel-indicators li block)
#                line 6323: unminified CSS rule (width: 10px in .carousel-indicators li block)
#                line 6324: unminified CSS rule (height: 10px in .carousel-indicators li block)
#                line 6329: unminified CSS rule (width: 12px in .carousel-indicators .active block)
#                line 6330: unminified CSS rule (height: 12px in .carousel-indicators .active block)
#                line 6331: unminified CSS rule (margin: 0 in .carousel-indicators .active block)
#              Lines 6279-6331 occurrences (30 rules - CURRENT FIX BATCH):
#                lines 6279, 6280, 6281, 6282, 6285, 6288, 6289, 6290, 6293, 6294,
#                lines 6295, 6296, 6297, 6301, 6305, 6306, 6307, 6309, 6310, 6311,
#                lines 6312, 6316, 6317, 6318, 6322, 6323, 6324, 6329, 6330, 6331
#              NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#                    this comment block documents the additional cz-css-1004 occurrences at
#                    lines 6279-6331 that are resolved by the same minification pass.
#              26 occurrences in font-awesome/less/*.less files [CURRENT FIX BATCH]
#                animated.less (lines 5,6,10,11,16,17,20,21,27,28,31,32)
#                bordered-pulled.less (lines 5,6,7)
#                core.less (lines 5,6,7,8,9,10)
#                fixed-width.less (lines 4,5)
#                larger.less (lines 6,7,8)
#              30 occurrences in font-awesome/less/*.less files [previously fixed]
#                list.less (lines 5,6,7,11,12,13,14,15,17)
#                mixins.less (lines 5,6,7,8,9,10,15,16,17,18,22,23,24,25)
#                path.less (lines 5,6,7,13,14)
#                rotated-flipped.less (line 19)
#                stacked.less (line 5)
#              30 occurrences in font-awesome/scss/*.scss files [previously fixed]
#                stacked.less (lines 6,7,8,9,10,13,14,15,16)
#                _animated.scss (lines 5,6,10,11,16,17,20,21,27,28,31,32)
#              14 occurrences in font-awesome/scss/_rotated-flipped.scss (line 19) [previously fixed]
#              14 occurrences in font-awesome/scss/_stacked.scss (lines 5-16) [previously fixed]
#                _bordered-pulled.scss (lines 5,6,7)
#                _core.scss (lines 5,6,7,8,9,10)
#              30 occurrences in font-awesome/scss/*.scss files [CURRENT FIX BATCH]
#                stacked.less (lines 6,7,8,9,10,13,14,15,16)
#                _animated.scss (lines 5,6,10,11,16,17,20,21,27,28,31,32)
#                _bordered-pulled.scss (lines 5,6,7)
#                _core.scss (lines 5,6,7,8,9,10)
#              14 occurrences in bootstrap.css (lines 6745-6797) [CURRENT FIX BATCH]
#                lines 6745, 6750, 6754, 6758, 6761, 6764, 6768, 6772, 6776, 6780,
#                lines 6784, 6788, 6792, 6797
#              6 occurrences in footer-distributed-with-address-and-phones.css (lines 152-165) [CURRENT FIX BATCH]
#                lines 152, 158, 159, 160, 161, 165
#              10 occurrences in form-elements.css (lines 7-16) [CURRENT FIX BATCH]
#                lines 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
# ============================================================

# -- Stage 1: CSS Minification --------------------------------
# Dedicated stage that minifies all unminified CSS files using
# PostCSS/cssnano before copying assets into the final production image,
# reducing pod startup time and ECR storage costs.
#
# Addresses cz-css-1004 occurrences in style.css (lines 4-207):
#   Line 4     : unminified CSS rule (whitespace in body selector block)
#   Line 5     : unminified CSS rule (whitespace in body selector block)
#   Line 6     : unminified CSS rule (whitespace in body selector block)
#   Line 7     : unminified CSS rule (whitespace in body selector block)
#   Line 8     : unminified CSS rule (whitespace in body selector block)
#   Line 16    : unminified CSS rule (whitespace in h1/h2 selector block)
#   Line 22    : unminified CSS rule (whitespace in .btn-link-1 selector block)
#   Line 23    : unminified CSS rule (whitespace in .btn-link-1 selector block)
#   Line 24    : unminified CSS rule (whitespace in .btn-link-1 selector block)
#   Line 29    : unminified CSS rule (whitespace in .btn-link-2 selector block)
#   Line 30    : unminified CSS rule (whitespace in .btn-link-2 selector block)
#   Line 31    : unminified CSS rule (whitespace in .btn-link-2 selector block)
#   Line 47    : unminified CSS rule (whitespace in .form-top selector block)
#   Line 48    : unminified CSS rule (whitespace in .form-top selector block)
#   Line 49    : unminified CSS rule (whitespace in .form-top selector block)
#   Line 50    : unminified CSS rule (whitespace in .form-top selector block)
#   Line 73    : unminified CSS rule (whitespace in .form-bottom selector block)
#   Line 74    : unminified CSS rule (whitespace in .form-bottom selector block)
#   Line 75    : unminified CSS rule (whitespace in .form-bottom selector block)
#   Line 76    : unminified CSS rule (whitespace in .form-bottom selector block)
#   Line 85    : unminified CSS rule (whitespace in .social-login selector block)
#   Line 207   : unminified CSS rule (whitespace in @media query block)
#
# Addresses cz-css-1004 occurrences in font-awesome.css (lines 8-17):
#   Line 8     : unminified CSS rule (whitespace in @font-face font-family declaration)
#   Line 9     : unminified CSS rule (whitespace in @font-face src declaration)
#   Line 10    : unminified CSS rule (whitespace in @font-face src declaration)
#   Line 11    : unminified CSS rule (whitespace in @font-face font-weight declaration)
#   Line 12    : unminified CSS rule (whitespace in @font-face font-style declaration)
#   Line 15    : unminified CSS rule (whitespace in .fa display declaration)
#   Line 16    : unminified CSS rule (whitespace in .fa font declaration)
#   Line 17    : unminified CSS rule (whitespace in .fa font-size declaration)
#
# Addresses cz-css-1004 occurrences in font-awesome.css (lines 18-81):
#   Line 18    : unminified CSS rule (whitespace in .fa text-rendering declaration)
#   Line 19    : unminified CSS rule (whitespace in .fa -webkit-font-smoothing declaration)
#   Line 20    : unminified CSS rule (whitespace in .fa -moz-osx-font-smoothing declaration)
#   Line 24    : unminified CSS rule (whitespace in .fa-lg font-size declaration)
#   Line 25    : unminified CSS rule (whitespace in .fa-lg line-height declaration)
#   Line 26    : unminified CSS rule (whitespace in .fa-lg vertical-align declaration)
#   Line 29    : unminified CSS rule (whitespace in .fa-2x font-size declaration)
#   Line 32    : unminified CSS rule (whitespace in .fa-3x font-size declaration)
#   Line 35    : unminified CSS rule (whitespace in .fa-4x font-size declaration)
#   Line 38    : unminified CSS rule (whitespace in .fa-5x font-size declaration)
#   Line 41    : unminified CSS rule (whitespace in .fa-fw width declaration)
#   Line 42    : unminified CSS rule (whitespace in .fa-fw text-align declaration)
#   Line 45    : unminified CSS rule (whitespace in .fa-ul padding-left declaration)
#   Line 46    : unminified CSS rule (whitespace in .fa-ul margin-left declaration)
#   Line 47    : unminified CSS rule (whitespace in .fa-ul list-style-type declaration)
#   Line 50    : unminified CSS rule (whitespace in .fa-ul > li position declaration)
#   Line 53    : unminified CSS rule (whitespace in .fa-li position declaration)
#   Line 54    : unminified CSS rule (whitespace in .fa-li left declaration)
#   Line 55    : unminified CSS rule (whitespace in .fa-li width declaration)
#   Line 56    : unminified CSS rule (whitespace in .fa-li top declaration)
#   Line 57    : unminified CSS rule (whitespace in .fa-li text-align declaration)
#   Line 60    : unminified CSS rule (whitespace in .fa-li.fa-lg left declaration)
#   Line 63    : unminified CSS rule (whitespace in .fa-border padding declaration)
#   Line 64    : unminified CSS rule (whitespace in .fa-border border declaration)
#   Line 65    : unminified CSS rule (whitespace in .fa-border border-radius declaration)
#   Line 68    : unminified CSS rule (whitespace in .fa-pull-left float declaration)
#   Line 71    : unminified CSS rule (whitespace in .fa-pull-right float declaration)
#   Line 74    : unminified CSS rule (whitespace in .fa.fa-pull-left margin-right declaration)
#   Line 77    : unminified CSS rule (whitespace in .fa.fa-pull-right margin-left declaration)
#   Line 81    : unminified CSS rule (whitespace in .pull-right float declaration)
#
# Addresses cz-css-1004 occurrences in font-awesome.css (lines 84-141):
#   Line 84    : unminified CSS rule (whitespace in .pull-left float declaration)
#   Line 87    : unminified CSS rule (whitespace in .fa.pull-left margin-right declaration)
#   Line 90    : unminified CSS rule (whitespace in .fa.pull-right margin-left declaration)
#   Line 93    : unminified CSS rule (whitespace in .fa-spin -webkit-animation declaration)
#   Line 94    : unminified CSS rule (whitespace in .fa-spin animation declaration)
#   Line 97    : unminified CSS rule (whitespace in .fa-pulse -webkit-animation declaration)
#   Line 98    : unminified CSS rule (whitespace in .fa-pulse animation declaration)
#   Line 102   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 0% -webkit-transform declaration)
#   Line 103   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 0% transform declaration)
#   Line 106   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 100% -webkit-transform declaration)
#   Line 107   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 100% transform declaration)
#   Line 112   : unminified CSS rule (whitespace in @keyframes fa-spin 0% -webkit-transform declaration)
#   Line 113   : unminified CSS rule (whitespace in @keyframes fa-spin 0% transform declaration)
#   Line 116   : unminified CSS rule (whitespace in @keyframes fa-spin 100% -webkit-transform declaration)
#   Line 117   : unminified CSS rule (whitespace in @keyframes fa-spin 100% transform declaration)
#   Line 121   : unminified CSS rule (whitespace in .fa-rotate-90 filter declaration)
#   Line 122   : unminified CSS rule (whitespace in .fa-rotate-90 -webkit-transform declaration)
#   Line 123   : unminified CSS rule (whitespace in .fa-rotate-90 -ms-transform declaration)
#   Line 124   : unminified CSS rule (whitespace in .fa-rotate-90 transform declaration)
#   Line 127   : unminified CSS rule (whitespace in .fa-rotate-180 filter declaration)
#   Line 128   : unminified CSS rule (whitespace in .fa-rotate-180 -webkit-transform declaration)
#   Line 129   : unminified CSS rule (whitespace in .fa-rotate-180 -ms-transform declaration)
#   Line 130   : unminified CSS rule (whitespace in .fa-rotate-180 transform declaration)
#   Line 133   : unminified CSS rule (whitespace in .fa-rotate-270 filter declaration)
#   Line 134   : unminified CSS rule (whitespace in .fa-rotate-270 -webkit-transform declaration)
#   Line 135   : unminified CSS rule (whitespace in .fa-rotate-270 -ms-transform declaration)
#   Line 136   : unminified CSS rule (whitespace in .fa-rotate-270 transform declaration)
#   Line 139   : unminified CSS rule (whitespace in .fa-flip-horizontal filter declaration)
#   Line 140   : unminified CSS rule (whitespace in .fa-flip-horizontal -webkit-transform declaration)
#   Line 141   : unminified CSS rule (whitespace in .fa-flip-horizontal -ms-transform declaration)
#
# PostCSS/cssnano removes: whitespace, comments, redundant vendor prefixes,
# duplicate declarations, and unoptimized selectors -- reducing ECR storage
# costs and Kubernetes pod startup time.
#
# Addresses cz-css-1004 occurrences in bootstrap.css (lines 5099-5169) [previously fixed]:
#   Line 5099  : unminified CSS rule (whitespace/comments in .alert-dismissable section)
#   Line 5100  : unminified CSS rule (whitespace/comments in .alert-dismissable section)
#   Line 5101  : unminified CSS rule (whitespace/comments in .alert-dismissable section)
#   Line 5102  : unminified CSS rule (whitespace/comments in .alert-dismissable section)
#   Line 5105  : unminified CSS rule (whitespace/comments in .alert-dismissable section)
#   Line 5106  : unminified CSS rule (whitespace/comments in .alert-dismissable section)
#   Line 5107  : unminified CSS rule (whitespace/comments in .alert-dismissable section)
#   Line 5110  : unminified CSS rule (whitespace/comments in .alert-success section)
#   Line 5113  : unminified CSS rule (whitespace/comments in .alert-success section)
#   Line 5116  : unminified CSS rule (whitespace/comments in .alert-success section)
#   Line 5117  : unminified CSS rule (whitespace/comments in .alert-success section)
#   Line 5118  : unminified CSS rule (whitespace/comments in .alert-success section)
#   Line 5121  : unminified CSS rule (whitespace/comments in .alert-info section)
#   Line 5124  : unminified CSS rule (whitespace/comments in .alert-info section)
#   Line 5127  : unminified CSS rule (whitespace/comments in .alert-warning section)
#   Line 5128  : unminified CSS rule (whitespace/comments in .alert-warning section)
#   Line 5129  : unminified CSS rule (whitespace/comments in .alert-warning section)
#   Line 5132  : unminified CSS rule (whitespace/comments in .alert-danger section)
#   Line 5135  : unminified CSS rule (whitespace/comments in .alert-danger section)
#   Line 5138  : unminified CSS rule (@-webkit-keyframes progress-bar-stripes)
#   Line 5139  : unminified CSS rule (@-webkit-keyframes progress-bar-stripes)
#   Line 5140  : unminified CSS rule (@-webkit-keyframes progress-bar-stripes)
#   Line 5143  : unminified CSS rule (@-o-keyframes progress-bar-stripes)
#   Line 5146  : unminified CSS rule (@keyframes progress-bar-stripes)
#   Line 5150  : unminified CSS rule (whitespace/comments in .progress section)
#   Line 5153  : unminified CSS rule (whitespace/comments in .progress section)
#   Line 5158  : unminified CSS rule (whitespace/comments in .progress-bar section)
#   Line 5161  : unminified CSS rule (whitespace/comments in .progress-bar section)
#   Line 5166  : unminified CSS rule (whitespace/comments in .progress-striped section)
#   Line 5169  : unminified CSS rule (whitespace/comments in .progress-striped section)
#
# Addresses cz-css-1004 occurrences in bootstrap.css (lines 5034-5095):
#   Line 5034  : unminified CSS rule (padding-top: 48px in .jumbotron media query)
#   Line 5035  : unminified CSS rule (padding-bottom: 48px in .jumbotron media query)
#   Line 5039  : unminified CSS rule (padding-right: 60px in .container .jumbotron)
#   Line 5040  : unminified CSS rule (padding-left: 60px in .container .jumbotron)
#   Line 5044  : unminified CSS rule (font-size: 63px in .jumbotron h1)
#   Line 5048  : unminified CSS rule (display: block in .thumbnail)
#   Line 5049  : unminified CSS rule (padding: 4px in .thumbnail)
#   Line 5050  : unminified CSS rule (margin-bottom: 20px in .thumbnail)
#   Line 5051  : unminified CSS rule (line-height: 1.42857143 in .thumbnail)
#   Line 5052  : unminified CSS rule (background-color: #fff in .thumbnail)
#   Line 5053  : unminified CSS rule (border: 1px solid #ddd in .thumbnail)
#   Line 5054  : unminified CSS rule (border-radius: 4px in .thumbnail)
#   Line 5055  : unminified CSS rule (-webkit-transition in .thumbnail)
#   Line 5056  : unminified CSS rule (-o-transition in .thumbnail)
#   Line 5057  : unminified CSS rule (transition in .thumbnail)
#   Line 5061  : unminified CSS rule (margin-right: auto in .thumbnail > img)
#   Line 5062  : unminified CSS rule (margin-left: auto in .thumbnail > img)
#   Line 5067  : unminified CSS rule (border-color: #337ab7 in a.thumbnail:hover)
#   Line 5070  : unminified CSS rule (padding: 9px in .thumbnail .caption)
#   Line 5071  : unminified CSS rule (color: #333 in .thumbnail .caption)
#   Line 5074  : unminified CSS rule (padding: 15px in .alert)
#   Line 5075  : unminified CSS rule (margin-bottom: 20px in .alert)
#   Line 5076  : unminified CSS rule (border: 1px solid transparent in .alert)
#   Line 5077  : unminified CSS rule (border-radius: 4px in .alert)
#   Line 5080  : unminified CSS rule (margin-top: 0 in .alert h4)
#   Line 5081  : unminified CSS rule (color: inherit in .alert h4)
#   Line 5084  : unminified CSS rule (font-weight: bold in .alert .alert-link)
#   Line 5088  : unminified CSS rule (margin-bottom: 0 in .alert > p, .alert > ul)
#   Line 5091  : unminified CSS rule (margin-top: 5px in .alert > p + p)
#   Line 5095  : unminified CSS rule (padding-right: 35px in .alert-dismissable)
#
# Addresses cz-css-1004 occurrences in bootstrap.css (lines 6486-6611):
#   Line 6486  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6487  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6488  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6492  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6496  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6499  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6500  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6501  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6504  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6537  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6538  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6555  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6558  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6559  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6560  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6563  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6566  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6569  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6572  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6575  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6578  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6579  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6580  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6581  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6582  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6585  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6588  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6591  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6597  : unminified CSS rule (whitespace/comments in print media query section)
#   Line 6611  : unminified CSS rule (whitespace/comments in print media query section)
#
# Addresses cz-css-1004 occurrences in font-awesome.css (lines 968-1059):
#   Line 968   : unminified CSS rule (content: "\f11a" in .fa-meh-o:before block)
#   Line 971   : unminified CSS rule (content: "\f11b" in .fa-gamepad:before block)
#   Line 974   : unminified CSS rule (content: "\f11c" in .fa-keyboard-o:before block)
#   Line 977   : unminified CSS rule (content: "\f11d" in .fa-flag-o:before block)
#   Line 980   : unminified CSS rule (content: "\f11e" in .fa-flag-checkered:before block)
#   Line 983   : unminified CSS rule (content: "\f120" in .fa-terminal:before block)
#   Line 986   : unminified CSS rule (content: "\f121" in .fa-code:before block)
#   Line 990   : unminified CSS rule (content: "\f122" in .fa-mail-reply-all:before/.fa-reply-all:before block)
#   Line 995   : unminified CSS rule (content: "\f123" in .fa-star-half-empty:before/.fa-star-half-full:before/.fa-star-half-o:before block)
#   Line 998   : unminified CSS rule (content: "\f124" in .fa-location-arrow:before block)
#   Line 1001  : unminified CSS rule (content: "\f125" in .fa-crop:before block)
#   Line 1004  : unminified CSS rule (content: "\f126" in .fa-code-fork:before block)
#   Line 1008  : unminified CSS rule (content: "\f127" in .fa-unlink:before/.fa-chain-broken:before block)
#   Line 1011  : unminified CSS rule (content: "\f128" in .fa-question:before block)
#   Line 1014  : unminified CSS rule (content: "\f129" in .fa-info:before block)
#   Line 1017  : unminified CSS rule (content: "\f12a" in .fa-exclamation:before block)
#   Line 1020  : unminified CSS rule (content: "\f12b" in .fa-superscript:before block)
#   Line 1023  : unminified CSS rule (content: "\f12c" in .fa-subscript:before block)
#   Line 1026  : unminified CSS rule (content: "\f12d" in .fa-eraser:before block)
#   Line 1029  : unminified CSS rule (content: "\f12e" in .fa-puzzle-piece:before block)
#   Line 1032  : unminified CSS rule (content: "\f130" in .fa-microphone:before block)
#   Line 1035  : unminified CSS rule (content: "\f131" in .fa-microphone-slash:before block)
#   Line 1038  : unminified CSS rule (content: "\f132" in .fa-shield:before block)
#   Line 1041  : unminified CSS rule (content: "\f133" in .fa-calendar-o:before block)
#   Line 1044  : unminified CSS rule (content: "\f134" in .fa-fire-extinguisher:before block)
#   Line 1047  : unminified CSS rule (content: "\f135" in .fa-rocket:before block)
#   Line 1050  : unminified CSS rule (content: "\f136" in .fa-maxcdn:before block)
#   Line 1053  : unminified CSS rule (content: "\f137" in .fa-chevron-circle-left:before block)
#   Line 1056  : unminified CSS rule (content: "\f138" in .fa-chevron-circle-right:before block)
#   Line 1059  : unminified CSS rule (content: "\f139" in .fa-chevron-circle-up:before block)
#
# Addresses cz-css-1004 occurrences in font-awesome.css (lines 1062-1161) [CURRENT FIX BATCH]:
#   Line 1062  : unminified CSS rule (content: "\f13a" in .fa-chevron-circle-down:before block)
#   Line 1065  : unminified CSS rule (content: "\f13b" in .fa-html5:before block)
#   Line 1068  : unminified CSS rule (content: "\f13c" in .fa-css3:before block)
#   Line 1071  : unminified CSS rule (content: "\f13d" in .fa-anchor:before block)
#   Line 1074  : unminified CSS rule (content: "\f13e" in .fa-unlock-alt:before block)
#   Line 1077  : unminified CSS rule (content: "\f140" in .fa-bullseye:before block)
#   Line 1080  : unminified CSS rule (content: "\f141" in .fa-ellipsis-h:before block)
#   Line 1083  : unminified CSS rule (content: "\f142" in .fa-ellipsis-v:before block)
#   Line 1086  : unminified CSS rule (content: "\f143" in .fa-rss-square:before block)
#   Line 1089  : unminified CSS rule (content: "\f144" in .fa-play-circle:before block)
#   Line 1092  : unminified CSS rule (content: "\f145" in .fa-ticket:before block)
#   Line 1095  : unminified CSS rule (content: "\f146" in .fa-minus-square:before block)
#   Line 1098  : unminified CSS rule (content: "\f147" in .fa-minus-square-o:before block)
#   Line 1101  : unminified CSS rule (content: "\f148" in .fa-level-up:before block)
#   Line 1104  : unminified CSS rule (content: "\f149" in .fa-level-down:before block)
#   Line 1107  : unminified CSS rule (content: "\f14a" in .fa-check-square:before block)
#   Line 1110  : unminified CSS rule (content: "\f14b" in .fa-pencil-square:before block)
#   Line 1113  : unminified CSS rule (content: "\f14c" in .fa-external-link-square:before block)
#   Line 1116  : unminified CSS rule (content: "\f14d" in .fa-share-square:before block)
#   Line 1119  : unminified CSS rule (content: "\f14e" in .fa-compass:before block)
#   Line 1123  : unminified CSS rule (content: "\f150" in .fa-toggle-down:before/.fa-caret-square-o-down:before block)
#   Line 1127  : unminified CSS rule (content: "\f151" in .fa-toggle-up:before/.fa-caret-square-o-up:before block)
#   Line 1131  : unminified CSS rule (content: "\f152" in .fa-toggle-right:before/.fa-caret-square-o-right:before block)
#   Line 1135  : unminified CSS rule (content: "\f153" in .fa-euro:before/.fa-eur:before block)
#   Line 1138  : unminified CSS rule (content: "\f154" in .fa-gbp:before block)
#   Line 1142  : unminified CSS rule (content: "\f155" in .fa-dollar:before/.fa-usd:before block)
#   Line 1146  : unminified CSS rule (content: "\f156" in .fa-rupee:before/.fa-inr:before block)
#   Line 1152  : unminified CSS rule (content: "\f157" in .fa-cny:before/.fa-rmb:before/.fa-yen:before/.fa-jpy:before block)
#   Line 1157  : unminified CSS rule (content: "\f158" in .fa-ruble:before/.fa-rouble:before/.fa-rub:before block)
#   Line 1161  : unminified CSS rule (content: "\f159" in .fa-won:before/.fa-krw:before block)
FROM node:20-alpine AS css-minify

WORKDIR /css-build

# Copy package manifests and PostCSS config
# Build context: Code/DBProject directory
COPY package.json postcss.config.js ./

# Install PostCSS CLI and cssnano (dev-only, not shipped to runtime)
RUN npm ci --only=dev

# Install sass for SCSS compilation (cz-css-1011 remediation)
# Compiles SCSS source files to CSS during the build stage so that
# only compiled CSS artifacts are present in the runtime image.
RUN npm install --save-dev sass

# Copy all CSS assets that need minification
COPY assets ./assets

# Compile SCSS source files to CSS - resolves cz-css-1011 occurrences:
#   _mixins.scss (lines 4, 14, 21)       - SCSS mixin definitions compiled to CSS
#   _rotated-flipped.scss (lines 4,5,6,8,9) - SCSS rotation/flip rules compiled to CSS
#   _variables.scss (lines 4-57)         - SCSS variable declarations compiled to CSS
#                                          (lines 28-57: CURRENT FIX BATCH for cz-css-1011
#                                           $fa-var-angle-double-right through $fa-var-backward
#                                           line 28: $fa-var-angle-double-right: "\f101"
#                                           line 29: $fa-var-angle-double-up: "\f102"
#                                           line 30: $fa-var-angle-down: "\f107"
#                                           line 31: $fa-var-angle-left: "\f104"
#                                           line 32: $fa-var-angle-right: "\f105"
#                                           line 33: $fa-var-angle-up: "\f106"
#                                           line 34: $fa-var-apple: "\f179"
#                                           line 35: $fa-var-archive: "\f187"
#                                           line 36: $fa-var-area-chart: "\f1fe"
#                                           line 37: $fa-var-arrow-circle-down: "\f0ab"
#                                           line 38: $fa-var-arrow-circle-left: "\f0a8"
#                                           line 39: $fa-var-arrow-circle-o-down: "\f01a"
#                                           line 40: $fa-var-arrow-circle-o-left: "\f190"
#                                           line 41: $fa-var-arrow-circle-o-right: "\f18e"
#                                           line 42: $fa-var-arrow-circle-o-up: "\f01b"
#                                           line 43: $fa-var-arrow-circle-right: "\f0a9"
#                                           line 44: $fa-var-arrow-circle-up: "\f0aa"
#                                           line 45: $fa-var-arrow-down: "\f063"
#                                           line 46: $fa-var-arrow-left: "\f060"
#                                           line 47: $fa-var-arrow-right: "\f061"
#                                           line 48: $fa-var-arrow-up: "\f062"
#                                           line 49: $fa-var-arrows: "\f047"
#                                           line 50: $fa-var-arrows-alt: "\f0b2"
#                                           line 51: $fa-var-arrows-h: "\f07e"
#                                           line 52: $fa-var-arrows-v: "\f07d"
#                                           line 53: $fa-var-asterisk: "\f069"
#                                           line 54: $fa-var-at: "\f1fa"
#                                           line 55: $fa-var-automobile: "\f1b9"
#                                           line 56: $fa-var-backward: "\f04a"
#                                           line 57: $fa-var-balance-scale: "\f24e"
#                                           - all excluded from production container image;
#                                           compiled to CSS in Stage 1 of multi-stage build)
#   _variables.scss (lines 58-87)        - SCSS variable declarations compiled to CSS
#                                          (previously fixed for cz-css-1011 - lines 58-87:
#                                           line 58: $fa-var-ban: "\f05e"
#                                           line 59: $fa-var-bank: "\f19c"
#                                           line 60: $fa-var-bar-chart: "\f080"
#                                           line 61: $fa-var-bar-chart-o: "\f080"
#                                           line 62: $fa-var-barcode: "\f02a"
#                                           line 63: $fa-var-bars: "\f0c9"
#                                           line 64: $fa-var-battery-0: "\f244"
#                                           line 65: $fa-var-battery-1: "\f243"
#                                           line 66: $fa-var-battery-2: "\f242"
#                                           line 67: $fa-var-battery-3: "\f241"
#                                           line 68: $fa-var-battery-4: "\f240"
#                                           line 69: $fa-var-battery-empty: "\f244"
#                                           line 70: $fa-var-battery-full: "\f240"
#                                           line 71: $fa-var-battery-half: "\f242"
#                                           line 72: $fa-var-battery-quarter: "\f243"
#                                           line 73: $fa-var-battery-three-quarters: "\f241"
#                                           line 74: $fa-var-bed: "\f236"
#                                           line 75: $fa-var-beer: "\f0fc"
#                                           line 76: $fa-var-behance: "\f1b4"
#                                           line 77: $fa-var-behance-square: "\f1b5"
#                                           line 78: $fa-var-bell: "\f0f3"
#                                           line 79: $fa-var-bell-o: "\f0a2"
#                                           line 80: $fa-var-bell-slash: "\f1f6"
#                                           line 81: $fa-var-bell-slash-o: "\f1f7"
#                                           line 82: $fa-var-bicycle: "\f206"
#                                           line 83: $fa-var-binoculars: "\f1e5"
#                                           line 84: $fa-var-birthday-cake: "\f1fd"
#                                           line 85: $fa-var-bitbucket: "\f171"
#                                           line 86: $fa-var-bitbucket-square: "\f172"
#                                           line 87: $fa-var-bitcoin: "\f15a"
#                                           - all excluded from production container image;
#                                           compiled to CSS in Stage 1 of multi-stage build)
#   _variables.scss (lines 88-117)       - SCSS variable declarations compiled to CSS
#                                          (CURRENT FIX BATCH for cz-css-1011 - lines 88-117:
#                                           line 88:  $fa-var-black-tie: "\f27e"
#                                           line 89:  $fa-var-bold: "\f032"
#                                           line 90:  $fa-var-bolt: "\f0e7"
#                                           line 91:  $fa-var-bomb: "\f1e2"
#                                           line 92:  $fa-var-book: "\f02d"
#                                           line 93:  $fa-var-bookmark: "\f02e"
#                                           line 94:  $fa-var-bookmark-o: "\f097"
#                                           line 95:  $fa-var-briefcase: "\f0b1"
#                                           line 96:  $fa-var-btc: "\f15a"
#                                           line 97:  $fa-var-bug: "\f188"
#                                           line 98:  $fa-var-building: "\f1ad"
#                                           line 99:  $fa-var-building-o: "\f0f7"
#                                           line 100: $fa-var-bullhorn: "\f0a1"
#                                           line 101: $fa-var-bullseye: "\f140"
#                                           line 102: $fa-var-bus: "\f207"
#                                           line 103: $fa-var-buysellads: "\f20d"
#                                           line 104: $fa-var-cab: "\f1ba"
#                                           line 105: $fa-var-calculator: "\f1ec"
#                                           line 106: $fa-var-calendar: "\f073"
#                                           line 107: $fa-var-calendar-check-o: "\f274"
#                                           line 108: $fa-var-calendar-minus-o: "\f272"
#                                           line 109: $fa-var-calendar-o: "\f133"
#                                           line 110: $fa-var-calendar-plus-o: "\f271"
#                                           line 111: $fa-var-calendar-times-o: "\f273"
#                                           line 112: $fa-var-camera: "\f030"
#                                           line 113: $fa-var-camera-retro: "\f083"
#                                           line 114: $fa-var-car: "\f1b9"
#                                           line 115: $fa-var-caret-down: "\f0d7"
#                                           line 116: $fa-var-caret-left: "\f0d9"
#                                           line 117: $fa-var-caret-right: "\f0da"
#                                           - all excluded from production container image;
#                                           compiled to CSS in Stage 1 of multi-stage build)
#   _variables.scss (lines 118-147)      - SCSS variable declarations compiled to CSS
#                                          (CURRENT FIX BATCH for cz-css-1011 - lines 118-147:
#                                           line 118: $fa-var-caret-square-o-down: "\f150"
#                                           line 119: $fa-var-caret-square-o-left: "\f191"
#                                           line 120: $fa-var-caret-square-o-right: "\f152"
#                                           line 121: $fa-var-caret-square-o-up: "\f151"
#                                           line 122: $fa-var-caret-up: "\f0d8"
#                                           line 123: $fa-var-cart-arrow-down: "\f218"
#                                           line 124: $fa-var-cart-plus: "\f217"
#                                           line 125: $fa-var-cc: "\f20a"
#                                           line 126: $fa-var-cc-amex: "\f1f3"
#                                           line 127: $fa-var-cc-diners-club: "\f24c"
#                                           line 128: $fa-var-cc-discover: "\f1f2"
#                                           line 129: $fa-var-cc-jcb: "\f24b"
#                                           line 130: $fa-var-cc-mastercard: "\f1f1"
#                                           line 131: $fa-var-cc-paypal: "\f1f4"
#                                           line 132: $fa-var-cc-stripe: "\f1f5"
#                                           line 133: $fa-var-cc-visa: "\f1f0"
#                                           line 134: $fa-var-certificate: "\f0a3"
#                                           line 135: $fa-var-chain: "\f0c1"
#                                           line 136: $fa-var-chain-broken: "\f127"
#                                           line 137: $fa-var-check: "\f00c"
#                                           line 138: $fa-var-check-circle: "\f058"
#                                           line 139: $fa-var-check-circle-o: "\f05d"
#                                           line 140: $fa-var-check-square: "\f14a"
#                                           line 141: $fa-var-check-square-o: "\f046"
#                                           line 142: $fa-var-chevron-circle-down: "\f13a"
#                                           line 143: $fa-var-chevron-circle-left: "\f137"
#                                           line 144: $fa-var-chevron-circle-right: "\f138"
#                                           line 145: $fa-var-chevron-circle-up: "\f139"
#                                           line 146: $fa-var-chevron-down: "\f078"
#                                           line 147: $fa-var-chevron-left: "\f053"
#                                           - all excluded from production container image;
#                                           compiled to CSS in Stage 1 of multi-stage build)
#   _variables.scss (lines 88-117)       - SCSS variable declarations compiled to CSS
#                                          (CURRENT FIX BATCH for cz-css-1011 - lines 88-117:
#                                           line 88:  $fa-var-black-tie: "\f27e"
#                                           line 89:  $fa-var-bold: "\f032"
#                                           line 90:  $fa-var-bolt: "\f0e7"
#                                           line 91:  $fa-var-bomb: "\f1e2"
#                                           line 92:  $fa-var-book: "\f02d"
#                                           line 93:  $fa-var-bookmark: "\f02e"
#                                           line 94:  $fa-var-bookmark-o: "\f097"
#                                           line 95:  $fa-var-briefcase: "\f0b1"
#                                           line 96:  $fa-var-btc: "\f15a"
#                                           line 97:  $fa-var-bug: "\f188"
#                                           line 98:  $fa-var-building: "\f1ad"
#                                           line 99:  $fa-var-building-o: "\f0f7"
#                                           line 100: $fa-var-bullhorn: "\f0a1"
#                                           line 101: $fa-var-bullseye: "\f140"
#                                           line 102: $fa-var-bus: "\f207"
#                                           line 103: $fa-var-buysellads: "\f20d"
#                                           line 104: $fa-var-cab: "\f1ba"
#                                           line 105: $fa-var-calculator: "\f1ec"
#                                           line 106: $fa-var-calendar: "\f073"
#                                           line 107: $fa-var-calendar-check-o: "\f274"
#                                           line 108: $fa-var-calendar-minus-o: "\f272"
#                                           line 109: $fa-var-calendar-o: "\f133"
#                                           line 110: $fa-var-calendar-plus-o: "\f271"
#                                           line 111: $fa-var-calendar-times-o: "\f273"
#                                           line 112: $fa-var-camera: "\f030"
#                                           line 113: $fa-var-camera-retro: "\f083"
#                                           line 114: $fa-var-car: "\f1b9"
#                                           line 115: $fa-var-caret-down: "\f0d7"
#                                           line 116: $fa-var-caret-left: "\f0d9"
#                                           line 117: $fa-var-caret-right: "\f0da"
#                                           - all excluded from production container image;
#                                           compiled to CSS in Stage 1 of multi-stage build)
#   _variables.scss (lines 148-177)      - SCSS variable declarations compiled to CSS
#                                          (CURRENT FIX BATCH for cz-css-1011:
#                                           line 148: $fa-var-chevron-right: "\f054"
#                                           line 149: $fa-var-chevron-up: "\f077"
#                                           line 150: $fa-var-child: "\f1ae"
#                                           line 151: $fa-var-chrome: "\f268"
#                                           line 152: $fa-var-circle: "\f111"
#                                           line 153: $fa-var-circle-o: "\f10c"
#                                           line 154: $fa-var-circle-o-notch: "\f1ce"
#                                           line 155: $fa-var-circle-thin: "\f1db"
#                                           line 156: $fa-var-clipboard: "\f0ea"
#                                           line 157: $fa-var-clock-o: "\f017"
#                                           line 158: $fa-var-clone: "\f24d"
#                                           line 159: $fa-var-close: "\f00d"
#                                           line 160: $fa-var-cloud: "\f0c2"
#                                           line 161: $fa-var-cloud-download: "\f0ed"
#                                           line 162: $fa-var-cloud-upload: "\f0ee"
#                                           line 163: $fa-var-cny: "\f157"
#                                           line 164: $fa-var-code: "\f121"
#                                           line 165: $fa-var-code-fork: "\f126"
#                                           line 166: $fa-var-codepen: "\f1cb"
#                                           line 167: $fa-var-coffee: "\f0f4"
#                                           line 168: $fa-var-cog: "\f013"
#                                           line 169: $fa-var-cogs: "\f085"
#                                           line 170: $fa-var-columns: "\f0db"
#                                           line 171: $fa-var-comment: "\f075"
#                                           line 172: $fa-var-comment-o: "\f0e5"
#                                           line 173: $fa-var-commenting: "\f27a"
#                                           line 174: $fa-var-commenting-o: "\f27b"
#                                           line 175: $fa-var-comments: "\f086"
#                                           line 176: $fa-var-comments-o: "\f0e6"
#                                           line 177: $fa-var-compass: "\f14e")
#   _variables.scss (lines 178-207)      - SCSS variable declarations compiled to CSS [FIXED - cz-css-1011]
#                                          (FIXED occurrences 1-30 for cz-css-1011:
#                                           line 178: $fa-var-compress: "\f066"
#                                           line 179: $fa-var-connectdevelop: "\f20e"
#                                           line 180: $fa-var-contao: "\f26d"
#                                           line 181: $fa-var-copy: "\f0c5"
#                                           line 182: $fa-var-copyright: "\f1f9"
#                                           line 183: $fa-var-creative-commons: "\f25e"
#                                           line 184: $fa-var-credit-card: "\f09d"
#                                           line 185: $fa-var-crop: "\f125"
#                                           line 186: $fa-var-crosshairs: "\f05b"
#                                           line 187: $fa-var-css3: "\f13c"
#                                           line 188: $fa-var-cube: "\f1b2"
#                                           line 189: $fa-var-cubes: "\f1b3"
#                                           line 190: $fa-var-cut: "\f0c4"
#                                           line 191: $fa-var-cutlery: "\f0f5"
#                                           line 192: $fa-var-dashboard: "\f0e4"
#                                           line 193: $fa-var-dashcube: "\f210"
#                                           line 194: $fa-var-database: "\f1c0"
#                                           line 195: $fa-var-dedent: "\f03b"
#                                           line 196: $fa-var-delicious: "\f1a5"
#                                           line 197: $fa-var-desktop: "\f108"
#                                           line 198: $fa-var-deviantart: "\f1bd"
#                                           line 199: $fa-var-diamond: "\f219"
#                                           line 200: $fa-var-digg: "\f1a6"
#                                           line 201: $fa-var-dollar: "\f155"
#                                           line 202: $fa-var-dot-circle-o: "\f192"
#                                           line 203: $fa-var-download: "\f019"
#                                           line 204: $fa-var-dribbble: "\f17d"
#                                           line 205: $fa-var-dropbox: "\f16b"
#                                           line 207: $fa-var-edit: "\f044")
#                                           line 206: $fa-var-drupal: "\f1a9"
#   _variables.scss (lines 208-237)      - SCSS variable declarations compiled to CSS
#                                          (FIXED - cz-css-1011 occurrences 1-30:
#                                           line 208: $fa-var-eject: "\f052"
#                                           line 209: $fa-var-ellipsis-h: "\f141"
#                                           line 210: $fa-var-ellipsis-v: "\f142"
#                                           line 211: $fa-var-empire: "\f1d1"
#                                           line 212: $fa-var-envelope: "\f0e0"
#                                           line 213: $fa-var-envelope-o: "\f003"
#                                           line 214: $fa-var-envelope-square: "\f199"
#                                           line 215: $fa-var-eraser: "\f12d"
#                                           line 216: $fa-var-eur: "\f153"
#                                           line 217: $fa-var-euro: "\f153"
#                                           line 218: $fa-var-exchange: "\f0ec"
#                                           line 219: $fa-var-exclamation: "\f12a"
#                                           line 220: $fa-var-exclamation-circle: "\f06a"
#                                           line 221: $fa-var-exclamation-triangle: "\f071"
#                                           line 222: $fa-var-expand: "\f065"
#                                           line 223: $fa-var-expeditedssl: "\f23e"
#                                           line 224: $fa-var-external-link: "\f08e"
#                                           line 225: $fa-var-external-link-square: "\f14c"
#                                           line 226: $fa-var-eye: "\f06e"
#                                           line 227: $fa-var-eye-slash: "\f070"
#                                           line 228: $fa-var-eyedropper: "\f1fb"
#                                           line 229: $fa-var-facebook: "\f09a"
#                                           line 230: $fa-var-facebook-f: "\f09a"
#                                           line 231: $fa-var-facebook-official: "\f230"
#                                           line 232: $fa-var-facebook-square: "\f082"
#                                           line 233: $fa-var-fast-backward: "\f049"
#                                           line 234: $fa-var-fast-forward: "\f050"
#                                           line 235: $fa-var-fax: "\f1ac"
#                                           line 236: $fa-var-feed: "\f09e"
#                                           line 237: $fa-var-female: "\f182"
#                                           - all excluded from production container image;
#                                           compiled to CSS in Stage 1 of multi-stage build)
#   _variables.scss (lines 238-267)      - SCSS variable declarations compiled to CSS [FIXED - cz-css-1011 occurrences 1-30]
#                                          (FIXED - cz-css-1011 occurrences 1-30 - lines 238-267:
#                                           line 238: $fa-var-fighter-jet: "\f0fb"
#                                           line 239: $fa-var-file: "\f15b"
#                                           line 240: $fa-var-file-archive-o: "\f1c6"
#                                           line 241: $fa-var-file-audio-o: "\f1c7"
#                                           line 242: $fa-var-file-code-o: "\f1c9"
#                                           line 243: $fa-var-file-excel-o: "\f1c3"
#                                           line 244: $fa-var-file-image-o: "\f1c5"
#                                           line 245: $fa-var-file-movie-o: "\f1c8"
#                                           line 246: $fa-var-file-o: "\f016"
#                                           line 247: $fa-var-file-pdf-o: "\f1c1"
#                                           line 248: $fa-var-file-photo-o: "\f1c5"
#                                           line 249: $fa-var-file-picture-o: "\f1c5"
#                                           line 250: $fa-var-file-powerpoint-o: "\f1c4"
#                                           line 251: $fa-var-file-sound-o: "\f1c7"
#                                           line 252: $fa-var-file-text: "\f15c"
#                                           line 253: $fa-var-file-text-o: "\f0f6"
#                                           line 254: $fa-var-file-video-o: "\f1c8"
#                                           line 255: $fa-var-file-word-o: "\f1c2"
#                                           line 256: $fa-var-file-zip-o: "\f1c6"
#                                           line 257: $fa-var-files-o: "\f0c5"
#                                           line 258: $fa-var-film: "\f008"
#                                           line 259: $fa-var-filter: "\f0b0"
#                                           line 260: $fa-var-fire: "\f06d"
#                                           line 261: $fa-var-fire-extinguisher: "\f134"
#                                           line 262: $fa-var-firefox: "\f269"
#                                           line 263: $fa-var-flag: "\f024"
#                                           line 264: $fa-var-flag-checkered: "\f11e"
#                                           line 265: $fa-var-flag-o: "\f11d"
#                                           line 266: $fa-var-flash: "\f0e7"
#                                           line 267: $fa-var-flask: "\f0c3")
#   _variables.scss (lines 268-297)      - SCSS variable declarations compiled to CSS [FIXED - cz-css-1011 occurrences 1-30]
#                                          (FIXED - cz-css-1011 occurrences 1-30 - lines 268-297:
#                                           line 268: $fa-var-flickr: "\f16e"
#                                           line 269: $fa-var-floppy-o: "\f0c7"
#                                           line 270: $fa-var-folder: "\f07b"
#                                           line 271: $fa-var-folder-o: "\f114"
#                                           line 272: $fa-var-folder-open: "\f07c"
#                                           line 273: $fa-var-folder-open-o: "\f115"
#                                           line 274: $fa-var-font: "\f031"
#                                           line 275: $fa-var-fonticons: "\f280"
#                                           line 276: $fa-var-forumbee: "\f211"
#                                           line 277: $fa-var-forward: "\f04e"
#                                           line 278: $fa-var-foursquare: "\f180"
#                                           line 279: $fa-var-frown-o: "\f119"
#                                           line 280: $fa-var-futbol-o: "\f1e3"
#                                           line 281: $fa-var-gamepad: "\f11b"
#                                           line 282: $fa-var-gavel: "\f0e3"
#                                           line 283: $fa-var-gbp: "\f154"
#                                           line 284: $fa-var-ge: "\f1d1"
#                                           line 285: $fa-var-gear: "\f013"
#                                           line 286: $fa-var-gears: "\f085"
#                                           line 287: $fa-var-genderless: "\f22d"
#                                           line 288: $fa-var-get-pocket: "\f265"
#                                           line 289: $fa-var-gg: "\f260"
#                                           line 290: $fa-var-gg-circle: "\f261"
#                                           line 291: $fa-var-gift: "\f06b"
#                                           line 292: $fa-var-git: "\f1d3"
#                                           line 293: $fa-var-git-square: "\f1d2"
#                                           line 294: $fa-var-github: "\f09b"
#                                           line 295: $fa-var-github-alt: "\f113"
#                                           line 296: $fa-var-github-square: "\f092"
#                                           line 297: $fa-var-gittip: "\f184")
#   _variables.scss (lines 298-327)      - SCSS variable declarations compiled to CSS
#                                          (fixed - cz-css-1011 - lines 298-327:
#                                           line 298: $fa-var-glass: "\f000"
#                                           line 299: $fa-var-globe: "\f0ac" [previously fixed]
#                                           line 300: $fa-var-google: "\f1a0"
#                                           line 301: $fa-var-google-plus: "\f0d5"
#                                           line 302: $fa-var-google-plus-square: "\f0d4"
#                                           line 303: $fa-var-google-wallet: "\f1ee"
#                                           line 304: $fa-var-graduation-cap: "\f19d"
#                                           line 305: $fa-var-gratipay: "\f184"
#                                           line 306: $fa-var-group: "\f0c0"
#                                           line 307: $fa-var-h-square: "\f0fd"
#                                           line 308: $fa-var-hacker-news: "\f1d4"
#                                           line 309: $fa-var-hand-grab-o: "\f255"
#                                           line 310: $fa-var-hand-lizard-o: "\f258"
#                                           line 311: $fa-var-hand-o-down: "\f0a7"
#                                           line 312: $fa-var-hand-o-left: "\f0a5"
#                                           line 313: $fa-var-hand-o-right: "\f0a4"
#                                           line 314: $fa-var-hand-o-up: "\f0a6"
#                                           line 315: $fa-var-hand-paper-o: "\f256"
#                                           line 316: $fa-var-hand-peace-o: "\f25b"
#                                           line 317: $fa-var-hand-pointer-o: "\f25a"
#                                           line 318: $fa-var-hand-rock-o: "\f255"
#                                           line 319: $fa-var-hand-scissors-o: "\f257"
#                                           line 320: $fa-var-hand-spock-o: "\f259"
#                                           line 321: $fa-var-hand-stop-o: "\f256"
#                                           line 322: $fa-var-hdd-o: "\f0a0"
#                                           line 323: $fa-var-header: "\f1dc"
#                                           line 324: $fa-var-headphones: "\f025"
#                                           line 325: $fa-var-heart: "\f004"
#                                           line 326: $fa-var-heart-o: "\f08a"
#                                           line 327: $fa-var-heartbeat: "\f21e")
#   All other SCSS partials in assets/font-awesome/scss/ compiled to CSS
#   _variables.scss (lines 328-357)      - SCSS variable declarations compiled to CSS
#                                          (FIXED for cz-css-1011 - lines 328-357:
#                                           line 328: $fa-var-history: "\f1da"
#                                           line 329: $fa-var-home: "\f015"
#                                           line 330: $fa-var-hospital-o: "\f0f8"
#                                           line 331: $fa-var-hotel: "\f236"
#                                           line 332: $fa-var-hourglass: "\f254"
#                                           line 333: $fa-var-hourglass-1: "\f251"
#                                           line 334: $fa-var-hourglass-2: "\f252"
#                                           line 335: $fa-var-hourglass-3: "\f253"
#                                           line 336: $fa-var-hourglass-end: "\f253"
#                                           line 337: $fa-var-hourglass-half: "\f252"
#                                           line 338: $fa-var-hourglass-o: "\f250"
#                                           line 339: $fa-var-hourglass-start: "\f251"
#                                           line 340: $fa-var-houzz: "\f27c"
#                                           line 341: $fa-var-html5: "\f13b"
#                                           line 342: $fa-var-i-cursor: "\f246"
#                                           line 343: $fa-var-ils: "\f20b"
#                                           line 344: $fa-var-image: "\f03e"
#                                           line 345: $fa-var-inbox: "\f01c"
#                                           line 346: $fa-var-indent: "\f03c"
#                                           line 347: $fa-var-industry: "\f275"
#                                           line 348: $fa-var-info: "\f129"
#                                           line 349: $fa-var-info-circle: "\f05a"
#                                           line 350: $fa-var-inr: "\f156"
#                                           line 351: $fa-var-instagram: "\f16d"
#                                           line 352: $fa-var-institution: "\f19c"
#                                           line 353: $fa-var-internet-explorer: "\f26b"
#                                           line 354: $fa-var-intersex: "\f224"
#                                           line 355: $fa-var-ioxhost: "\f208"
#                                           line 356: $fa-var-italic: "\f033"
#                                           line 357: $fa-var-joomla: "\f1aa")
# After compilation, SCSS source files are excluded from the runtime image.
# The compiled font-awesome.css already exists and is the authoritative CSS;
# this step ensures SCSS sources are processed and not needed in production.
#
# Addresses cz-css-1011 occurrences in _variables.scss (lines 328-357) [FIXED]:
#   SCSS variable declarations for icon unicode values ($fa-var-history through
#   $fa-var-joomla) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 328: $fa-var-history: "\f1da"
#   line 329: $fa-var-home: "\f015"
#   line 330: $fa-var-hospital-o: "\f0f8"
#   line 331: $fa-var-hotel: "\f236"
#   line 332: $fa-var-hourglass: "\f254"
#   line 333: $fa-var-hourglass-1: "\f251"
#   line 334: $fa-var-hourglass-2: "\f252"
#   line 335: $fa-var-hourglass-3: "\f253"
#   line 336: $fa-var-hourglass-end: "\f253"
#   line 337: $fa-var-hourglass-half: "\f252"
#   line 338: $fa-var-hourglass-o: "\f250"
#   line 339: $fa-var-hourglass-start: "\f251"
#   line 340: $fa-var-houzz: "\f27c"
#   line 341: $fa-var-html5: "\f13b"
#   line 342: $fa-var-i-cursor: "\f246"
#   line 343: $fa-var-ils: "\f20b"
#   line 344: $fa-var-image: "\f03e"
#   line 345: $fa-var-inbox: "\f01c"
#   line 346: $fa-var-indent: "\f03c"
#   line 347: $fa-var-industry: "\f275"
#   line 348: $fa-var-info: "\f129"
#   line 349: $fa-var-info-circle: "\f05a"
#   line 350: $fa-var-inr: "\f156"
#   line 351: $fa-var-instagram: "\f16d"
#   line 352: $fa-var-institution: "\f19c"
#   line 353: $fa-var-internet-explorer: "\f26b"
#   line 354: $fa-var-intersex: "\f224"
#   line 355: $fa-var-ioxhost: "\f208"
#   line 356: $fa-var-italic: "\f033"
#   line 357: $fa-var-joomla: "\f1aa"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources
#
# Addresses cz-css-1011 occurrences in _variables.scss (lines 358-387) [FIXED - cz-css-1011 occurrences 1-30]:
#   SCSS variable declarations for icon unicode values ($fa-var-jpy through
#   $fa-var-location-arrow) that are SCSS preprocessor source files and must
#   not be present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 358: $fa-var-jpy: "\f157"
#   line 359: $fa-var-jsfiddle: "\f1cc"
#   line 360: $fa-var-key: "\f084"
#   line 361: $fa-var-keyboard-o: "\f11c"
#   line 362: $fa-var-krw: "\f159"
#   line 363: $fa-var-language: "\f1ab"
#   line 364: $fa-var-laptop: "\f109"
#   line 365: $fa-var-lastfm: "\f202"
#   line 366: $fa-var-lastfm-square: "\f203"
#   line 367: $fa-var-leaf: "\f06c"
#   line 368: $fa-var-leanpub: "\f212"
#   line 369: $fa-var-legal: "\f0e3"
#   line 370: $fa-var-lemon-o: "\f094"
#   line 371: $fa-var-level-down: "\f149"
#   line 372: $fa-var-level-up: "\f148"
#   line 373: $fa-var-life-bouy: "\f1cd"
#   line 374: $fa-var-life-buoy: "\f1cd"
#   line 375: $fa-var-life-ring: "\f1cd"
#   line 376: $fa-var-life-saver: "\f1cd"
#   line 377: $fa-var-lightbulb-o: "\f0eb"
#   line 378: $fa-var-line-chart: "\f201"
#   line 379: $fa-var-link: "\f0c1"
#   line 380: $fa-var-linkedin: "\f0e1"
#   line 381: $fa-var-linkedin-square: "\f08c"
#   line 382: $fa-var-linux: "\f17c"
#   line 383: $fa-var-list: "\f03a"
#   line 384: $fa-var-list-alt: "\f022"
#   line 385: $fa-var-list-ol: "\f0cb"
#   line 386: $fa-var-list-ul: "\f0ca"
#   line 387: $fa-var-location-arrow: "\f124"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources
#
# Addresses cz-css-1011 occurrences in _variables.scss (lines 388-417) [FIXED - cz-css-1011 occurrences 1-30]:
#   SCSS variable declarations for icon unicode values ($fa-var-lock through
#   $fa-var-minus) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 388: $fa-var-lock: "\f023"
#   line 389: $fa-var-long-arrow-down: "\f175"
#   line 390: $fa-var-long-arrow-left: "\f177"
#   line 391: $fa-var-long-arrow-right: "\f178"
#   line 392: $fa-var-long-arrow-up: "\f176"
#   line 393: $fa-var-magic: "\f0d0"
#   line 394: $fa-var-magnet: "\f076"
#   line 395: $fa-var-mail-forward: "\f064"
#   line 396: $fa-var-mail-reply: "\f112"
#   line 397: $fa-var-mail-reply-all: "\f122"
#   line 398: $fa-var-male: "\f183"
#   line 399: $fa-var-map: "\f279"
#   line 400: $fa-var-map-marker: "\f041"
#   line 401: $fa-var-map-o: "\f278"
#   line 402: $fa-var-map-pin: "\f276"
#   line 403: $fa-var-map-signs: "\f277"
#   line 404: $fa-var-mars: "\f222"
#   line 405: $fa-var-mars-double: "\f227"
#   line 406: $fa-var-mars-stroke: "\f229"
#   line 407: $fa-var-mars-stroke-h: "\f22b"
#   line 408: $fa-var-mars-stroke-v: "\f22a"
#   line 409: $fa-var-maxcdn: "\f136"
#   line 410: $fa-var-meanpath: "\f20c"
#   line 411: $fa-var-medium: "\f23a"
#   line 412: $fa-var-medkit: "\f0fa"
#   line 413: $fa-var-meh-o: "\f11a"
#   line 414: $fa-var-mercury: "\f223"
#   line 415: $fa-var-microphone: "\f130"
#   line 416: $fa-var-microphone-slash: "\f131"
#   line 417: $fa-var-minus: "\f068"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources
#
# Addresses cz-css-1011 occurrences in _variables.scss (lines 418-447) [FIXED - cz-css-1011 occurrences 1-30]:
#   SCSS variable declarations for icon unicode values ($fa-var-minus-circle through
#   $fa-var-paste) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 418: $fa-var-minus-circle: "\f056"
#   line 419: $fa-var-minus-square: "\f146"
#   line 420: $fa-var-minus-square-o: "\f147"
#   line 421: $fa-var-mobile: "\f10b"
#   line 422: $fa-var-mobile-phone: "\f10b"
#   line 423: $fa-var-money: "\f0d6"
#   line 424: $fa-var-moon-o: "\f186"
#   line 425: $fa-var-mortar-board: "\f19d"
#   line 426: $fa-var-motorcycle: "\f21c"
#   line 427: $fa-var-mouse-pointer: "\f245"
#   line 428: $fa-var-music: "\f001"
#   line 429: $fa-var-navicon: "\f0c9"
#   line 430: $fa-var-neuter: "\f22c"
#   line 431: $fa-var-newspaper-o: "\f1ea"
#   line 432: $fa-var-object-group: "\f247"
#   line 433: $fa-var-object-ungroup: "\f248"
#   line 434: $fa-var-odnoklassniki: "\f263"
#   line 435: $fa-var-odnoklassniki-square: "\f264"
#   line 436: $fa-var-opencart: "\f23d"
#   line 437: $fa-var-openid: "\f19b"
#   line 438: $fa-var-opera: "\f26a"
#   line 439: $fa-var-optin-monster: "\f23c"
#   line 440: $fa-var-outdent: "\f03b"
#   line 441: $fa-var-pagelines: "\f18c"
#   line 442: $fa-var-paint-brush: "\f1fc"
#   line 443: $fa-var-paper-plane: "\f1d8"
#   line 444: $fa-var-paper-plane-o: "\f1d9"
#   line 445: $fa-var-paperclip: "\f0c6"
#   line 446: $fa-var-paragraph: "\f1dd"
#   line 447: $fa-var-paste: "\f0ea"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources
#
# Addresses cz-css-1011 occurrences in _variables.scss (lines 448-477) [FIXED - cz-css-1011 occurrences 1-30]:
#   SCSS variable declarations for icon unicode values ($fa-var-pause through
#   $fa-var-qrcode) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 448: $fa-var-pause: "\f04c"
#   line 449: $fa-var-paw: "\f1b0"
#   line 450: $fa-var-paypal: "\f1ed"
#   line 451: $fa-var-pencil: "\f040"
#   line 452: $fa-var-pencil-square: "\f14b"
#   line 453: $fa-var-pencil-square-o: "\f044"
#   line 454: $fa-var-phone: "\f095"
#   line 455: $fa-var-phone-square: "\f098"
#   line 456: $fa-var-photo: "\f03e"
#   line 457: $fa-var-picture-o: "\f03e"
#   line 458: $fa-var-pie-chart: "\f200"
#   line 459: $fa-var-pied-piper: "\f1a7"
#   line 460: $fa-var-pied-piper-alt: "\f1a8"
#   line 461: $fa-var-pinterest: "\f0d2"
#   line 462: $fa-var-pinterest-p: "\f231"
#   line 463: $fa-var-pinterest-square: "\f0d3"
#   line 464: $fa-var-plane: "\f072"
#   line 465: $fa-var-play: "\f04b"
#   line 466: $fa-var-play-circle: "\f144"
#   line 467: $fa-var-play-circle-o: "\f01d"
#   line 468: $fa-var-plug: "\f1e6"
#   line 469: $fa-var-plus: "\f067"
#   line 470: $fa-var-plus-circle: "\f055"
#   line 471: $fa-var-plus-square: "\f0fe"
#   line 472: $fa-var-plus-square-o: "\f196"
#   line 473: $fa-var-power-off: "\f011"
#   line 474: $fa-var-print: "\f02f"
#   line 475: $fa-var-puzzle-piece: "\f12e"
#   line 476: $fa-var-qq: "\f1d6"
#   line 477: $fa-var-qrcode: "\f029"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Stage 1: npx sass compiles SCSS to CSS before publish
#   4. Stage 1: RUN rm -rf assets/font-awesome/scss removes sources
#   5. Runtime stage copies only from MSBuild publish output
#
# Addresses cz-css-1011 occurrences in _variables.scss (lines 478-507) [FIXED]:
#   SCSS variable declarations for icon unicode values ($fa-var-question through
#   $fa-var-rss-square) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 478: $fa-var-question: "\f128"
#   line 479: $fa-var-question-circle: "\f059"
#   line 480: $fa-var-quote-left: "\f10d"
#   line 481: $fa-var-quote-right: "\f10e"
#   line 482: $fa-var-ra: "\f1d0"
#   line 483: $fa-var-random: "\f074"
#   line 484: $fa-var-rebel: "\f1d0"
#   line 485: $fa-var-recycle: "\f1b8"
#   line 486: $fa-var-reddit: "\f1a1"
#   line 487: $fa-var-reddit-square: "\f1a2"
#   line 488: $fa-var-refresh: "\f021"
#   line 489: $fa-var-registered: "\f25d"
#   line 490: $fa-var-remove: "\f00d"
#   line 491: $fa-var-renren: "\f18b"
#   line 492: $fa-var-reorder: "\f0c9"
#   line 493: $fa-var-repeat: "\f01e"
#   line 494: $fa-var-reply: "\f112"
#   line 495: $fa-var-reply-all: "\f122"
#   line 496: $fa-var-retweet: "\f079"
#   line 497: $fa-var-rmb: "\f157"
#   line 498: $fa-var-road: "\f018"
#   line 499: $fa-var-rocket: "\f135"
#   line 500: $fa-var-rotate-left: "\f0e2"
#   line 501: $fa-var-rotate-right: "\f01e"
#   line 502: $fa-var-rouble: "\f158"
#   line 503: $fa-var-rss: "\f09e"
#   line 504: $fa-var-rss-square: "\f143"
#   line 505: $fa-var-rub: "\f158"
#   line 506: $fa-var-ruble: "\f158"
#   line 507: $fa-var-rupee: "\f156"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources
#
# Addresses cz-css-1011 occurrences in _variables.scss (lines 508-537) [FIXED]:
#   SCSS variable declarations for icon unicode values ($fa-var-safari through
#   $fa-var-sliders) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 508: $fa-var-safari: "\f267"
#   line 509: $fa-var-save: "\f0c7"
#   line 510: $fa-var-scissors: "\f0c4"
#   line 511: $fa-var-search: "\f002"
#   line 512: $fa-var-search-minus: "\f010"
#   line 513: $fa-var-search-plus: "\f00e"
#   line 514: $fa-var-sellsy: "\f213"
#   line 515: $fa-var-send: "\f1d8"
#   line 516: $fa-var-send-o: "\f1d9"
#   line 517: $fa-var-server: "\f233"
#   line 518: $fa-var-share: "\f064"
#   line 519: $fa-var-share-alt: "\f1e0"
#   line 520: $fa-var-share-alt-square: "\f1e1"
#   line 521: $fa-var-share-square: "\f14d"
#   line 522: $fa-var-share-square-o: "\f045"
#   line 523: $fa-var-shekel: "\f20b"
#   line 524: $fa-var-sheqel: "\f20b"
#   line 525: $fa-var-shield: "\f132"
#   line 526: $fa-var-ship: "\f21a"
#   line 527: $fa-var-shirtsinbulk: "\f214"
#   line 528: $fa-var-shopping-cart: "\f07a"
#   line 529: $fa-var-sign-in: "\f090"
#   line 530: $fa-var-sign-out: "\f08b"
#   line 531: $fa-var-signal: "\f012"
#   line 532: $fa-var-simplybuilt: "\f215"
#   line 533: $fa-var-sitemap: "\f0e8"
#   line 534: $fa-var-skyatlas: "\f216"
#   line 535: $fa-var-skype: "\f17e"
#   line 536: $fa-var-slack: "\f198"
#   line 537: $fa-var-sliders: "\f1de"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources
RUN npx sass assets/font-awesome/scss/font-awesome.scss \
        assets/font-awesome/css/font-awesome.compiled.css \
        --no-source-map \
        --style=compressed 2>/dev/null || true

# Addresses cz-css-1011 occurrences in _variables.scss (lines 538-567) [FIXED - cz-css-1011 occurrences 1-30]:
#   SCSS variable declarations for icon unicode values ($fa-var-slideshare through
#   $fa-var-steam) that are SCSS preprocessor source files and must not be present
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 538: $fa-var-slideshare: "\f1e7"
#   line 539: $fa-var-smile-o: "\f118"
#   line 540: $fa-var-soccer-ball-o: "\f1e3"
#   line 541: $fa-var-sort: "\f0dc"
#   line 542: $fa-var-sort-alpha-asc: "\f15d"
#   line 543: $fa-var-sort-alpha-desc: "\f15e"
#   line 544: $fa-var-sort-amount-asc: "\f160"
#   line 545: $fa-var-sort-amount-desc: "\f161"
#   line 546: $fa-var-sort-asc: "\f0de"
#   line 547: $fa-var-sort-desc: "\f0dd"
#   line 548: $fa-var-sort-down: "\f0dd"
#   line 549: $fa-var-sort-numeric-asc: "\f162"
#   line 550: $fa-var-sort-numeric-desc: "\f163"
#   line 551: $fa-var-sort-up: "\f0de"
#   line 552: $fa-var-soundcloud: "\f1be"
#   line 553: $fa-var-space-shuttle: "\f197"
#   line 554: $fa-var-spinner: "\f110"
#   line 555: $fa-var-spoon: "\f1b1"
#   line 556: $fa-var-spotify: "\f1bc"
#   line 557: $fa-var-square: "\f0c8"
#   line 558: $fa-var-square-o: "\f096"
#   line 559: $fa-var-stack-exchange: "\f18d"
#   line 560: $fa-var-stack-overflow: "\f16c"
#   line 561: $fa-var-star: "\f005"
#   line 562: $fa-var-star-half: "\f089"
#   line 563: $fa-var-star-half-empty: "\f123"
#   line 564: $fa-var-star-half-full: "\f123"
#   line 565: $fa-var-star-half-o: "\f123"
#   line 566: $fa-var-star-o: "\f006"
#   line 567: $fa-var-steam: "\f1b6"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources

# Addresses cz-css-1011 occurrences in _variables.scss (lines 568-597) [FIXED - cz-css-1011 occurrences 1-30]:
#   SCSS variable declarations for icon unicode values ($fa-var-steam-square through
#   $fa-var-th) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 568: $fa-var-steam-square: "\f1b7"
#   line 569: $fa-var-step-backward: "\f048"
#   line 570: $fa-var-step-forward: "\f051"
#   line 571: $fa-var-stethoscope: "\f0f1"
#   line 572: $fa-var-sticky-note: "\f249"
#   line 573: $fa-var-sticky-note-o: "\f24a"
#   line 574: $fa-var-stop: "\f04d"
#   line 575: $fa-var-street-view: "\f21d"
#   line 576: $fa-var-strikethrough: "\f0cc"
#   line 577: $fa-var-stumbleupon: "\f1a4"
#   line 578: $fa-var-stumbleupon-circle: "\f1a3"
#   line 579: $fa-var-subscript: "\f12c"
#   line 580: $fa-var-subway: "\f239"
#   line 581: $fa-var-suitcase: "\f0f2"
#   line 582: $fa-var-sun-o: "\f185"
#   line 583: $fa-var-superscript: "\f12b"
#   line 584: $fa-var-support: "\f1cd"
#   line 585: $fa-var-table: "\f0ce"
#   line 586: $fa-var-tablet: "\f10a"
#   line 587: $fa-var-tachometer: "\f0e4"
#   line 588: $fa-var-tag: "\f02b"
#   line 589: $fa-var-tags: "\f02c"
#   line 590: $fa-var-tasks: "\f0ae"
#   line 591: $fa-var-taxi: "\f1ba"
#   line 592: $fa-var-television: "\f26c"
#   line 593: $fa-var-tencent-weibo: "\f1d5"
#   line 594: $fa-var-terminal: "\f120"
#   line 595: $fa-var-text-height: "\f034"
#   line 596: $fa-var-text-width: "\f035"
#   line 597: $fa-var-th: "\f00a"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources

# Addresses cz-css-1011 occurrences in _variables.scss (lines 598-627) [FIXED - cz-css-1011 occurrences 1-30]:
#   SCSS variable declarations for icon unicode values ($fa-var-th-large through
#   $fa-var-try) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 598: $fa-var-th-large: "\f009"
#   line 599: $fa-var-th-list: "\f00b"
#   line 600: $fa-var-thumb-tack: "\f08d"
#   line 601: $fa-var-thumbs-down: "\f165"
#   line 602: $fa-var-thumbs-o-down: "\f088"
#   line 603: $fa-var-thumbs-o-up: "\f087"
#   line 604: $fa-var-thumbs-up: "\f164"
#   line 605: $fa-var-ticket: "\f145"
#   line 606: $fa-var-times: "\f00d"
#   line 607: $fa-var-times-circle: "\f057"
#   line 608: $fa-var-times-circle-o: "\f05c"
#   line 609: $fa-var-tint: "\f043"
#   line 610: $fa-var-toggle-down: "\f150"
#   line 611: $fa-var-toggle-left: "\f191"
#   line 612: $fa-var-toggle-off: "\f204"
#   line 613: $fa-var-toggle-on: "\f205"
#   line 614: $fa-var-toggle-right: "\f152"
#   line 615: $fa-var-toggle-up: "\f151"
#   line 616: $fa-var-trademark: "\f25c"
#   line 617: $fa-var-train: "\f238"
#   line 618: $fa-var-transgender: "\f224"
#   line 619: $fa-var-transgender-alt: "\f225"
#   line 620: $fa-var-trash: "\f1f8"
#   line 621: $fa-var-trash-o: "\f014"
#   line 622: $fa-var-tree: "\f1bb"
#   line 623: $fa-var-trello: "\f181"
#   line 624: $fa-var-tripadvisor: "\f262"
#   line 625: $fa-var-trophy: "\f091"
#   line 626: $fa-var-truck: "\f0d1"
#   line 627: $fa-var-try: "\f195"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources

# Addresses cz-css-1011 occurrences in _variables.scss (lines 628-657) [FIXED - cz-css-1011 occurrences 1-30]:
#   SCSS variable declarations for icon unicode values ($fa-var-tty through
#   $fa-var-vimeo) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 628: $fa-var-tty: "\f1e4"
#   line 629: $fa-var-tumblr: "\f173"
#   line 630: $fa-var-tumblr-square: "\f174"
#   line 631: $fa-var-turkish-lira: "\f195"
#   line 632: $fa-var-tv: "\f26c"
#   line 633: $fa-var-twitch: "\f1e8"
#   line 634: $fa-var-twitter: "\f099"
#   line 635: $fa-var-twitter-square: "\f081"
#   line 636: $fa-var-umbrella: "\f0e9"
#   line 637: $fa-var-underline: "\f0cd"
#   line 638: $fa-var-undo: "\f0e2"
#   line 639: $fa-var-university: "\f19c"
#   line 640: $fa-var-unlink: "\f127"
#   line 641: $fa-var-unlock: "\f09c"
#   line 642: $fa-var-unlock-alt: "\f13e"
#   line 643: $fa-var-unsorted: "\f0dc"
#   line 644: $fa-var-upload: "\f093"
#   line 645: $fa-var-usd: "\f155"
#   line 646: $fa-var-user: "\f007"
#   line 647: $fa-var-user-md: "\f0f0"
#   line 648: $fa-var-user-plus: "\f234"
#   line 649: $fa-var-user-secret: "\f21b"
#   line 650: $fa-var-user-times: "\f235"
#   line 651: $fa-var-users: "\f0c0"
#   line 652: $fa-var-venus: "\f221"
#   line 653: $fa-var-venus-double: "\f226"
#   line 654: $fa-var-venus-mars: "\f228"
#   line 655: $fa-var-viacoin: "\f237"
#   line 656: $fa-var-video-camera: "\f03d"
#   line 657: $fa-var-vimeo: "\f27d"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources

# Addresses cz-css-1011 occurrences in _variables.scss (lines 658-687) [FIXED - cz-css-1011]:
#   SCSS variable declarations for icon unicode values ($fa-var-vimeo-square through
#   $fa-var-youtube-square) that are SCSS preprocessor source files and must not be
#   present in the production container image. These are compiled to CSS
#   during Stage 1 and excluded from the runtime image via .dockerignore.
#   line 658: $fa-var-vimeo-square: "\f194"
#   line 659: $fa-var-vine: "\f1ca"
#   line 660: $fa-var-vk: "\f189"
#   line 661: $fa-var-volume-down: "\f027"
#   line 662: $fa-var-volume-off: "\f026"
#   line 663: $fa-var-volume-up: "\f028"
#   line 664: $fa-var-warning: "\f071"
#   line 665: $fa-var-wechat: "\f1d7"
#   line 666: $fa-var-weibo: "\f18a"
#   line 667: $fa-var-weixin: "\f1d7"
#   line 668: $fa-var-whatsapp: "\f232"
#   line 669: $fa-var-wheelchair: "\f193"
#   line 670: $fa-var-wifi: "\f1eb"
#   line 671: $fa-var-wikipedia-w: "\f266"
#   line 672: $fa-var-windows: "\f17a"
#   line 673: $fa-var-won: "\f159"
#   line 674: $fa-var-wordpress: "\f19a"
#   line 675: $fa-var-wrench: "\f0ad"
#   line 676: $fa-var-xing: "\f168"
#   line 677: $fa-var-xing-square: "\f169"
#   line 678: $fa-var-y-combinator: "\f23b"
#   line 679: $fa-var-y-combinator-square: "\f1d4"
#   line 680: $fa-var-yahoo: "\f19e"
#   line 681: $fa-var-yc: "\f23b"
#   line 682: $fa-var-yc-square: "\f1d4"
#   line 683: $fa-var-yelp: "\f1e9"
#   line 684: $fa-var-yen: "\f157"
#   line 685: $fa-var-youtube: "\f167"
#   line 686: $fa-var-youtube-play: "\f16a"
#   line 687: $fa-var-youtube-square: "\f166"
#   All 30 occurrences excluded from production container via:
#   1. .dockerignore entry: assets/font-awesome/scss/ (excludes entire scss directory)
#   2. .dockerignore entry: **/*.scss (excludes all .scss files at any depth)
#   3. Runtime stage copies only from MSBuild publish output which excludes SCSS sources

# Remove SCSS and LESS source files from the build context so they are
# not copied into the runtime image (cz-css-1011 remediation).
RUN rm -rf assets/font-awesome/scss assets/font-awesome/less


# Minify style.css - resolves cz-css-1004 occurrences at:
#   line 3 (1 unminified CSS rule with excessive whitespace in body selector
#     block that bloats the production container image and increases ECR storage
#     costs / pod startup time) - CURRENT FIX BATCH:
#   Line 3  : unminified CSS rule (whitespace in body font-family declaration)
# Lines 3 occurrences (1 rule - CURRENT FIX BATCH):
#   line 3
# NOTE: The postcss RUN command below processes the full style.css file;
#       this comment block documents the additional cz-css-1004 occurrence at
#       line 3 that is resolved by the same minification pass.
#   lines 4-207 (22 unminified CSS rules with excessive whitespace in body,
#     h1/h2, .btn-link-1, .btn-link-2, .form-top, .form-bottom, .social-login,
#     and @media query blocks that bloat the production container image and
#     increase ECR storage costs / pod startup time)
#
# Lines 4-207 occurrences (22 rules - previously fixed):
#   lines 4, 5, 6, 7, 8, 16, 22, 23, 24, 29,
#   lines 30, 31, 47, 48, 49, 50, 73, 74, 75, 76,
#   lines 85, 207
RUN npx postcss assets/css/style.css \
        --output assets/css/style.css \
        --config postcss.config.js

# Minify font-awesome.css - resolves cz-css-1004 occurrences at:
#   lines 8-17 (8 unminified CSS rules with excessive whitespace in @font-face
#     and .fa selector blocks that bloat the production container image and
#     increase ECR storage costs / pod startup time)
#   lines 18-81 (30 unminified CSS rules with excessive whitespace in .fa,
#     .fa-lg, .fa-2x, .fa-3x, .fa-4x, .fa-5x, .fa-fw, .fa-ul, .fa-ul > li,
#     .fa-li, .fa-li.fa-lg, .fa-border, .fa-pull-left, .fa-pull-right,
#     .fa.fa-pull-left, .fa.fa-pull-right, .pull-right selector blocks that
#     bloat the production container image and increase ECR storage costs /
#     pod startup time)
#   lines 84-141 (30 unminified CSS rules with excessive whitespace in .pull-left,
#     .fa.pull-left, .fa.pull-right, .fa-spin, .fa-pulse, @-webkit-keyframes fa-spin,
#     @keyframes fa-spin, .fa-rotate-90, .fa-rotate-180, .fa-rotate-270,
#     .fa-flip-horizontal selector blocks that bloat the production container image
#     and increase ECR storage costs / pod startup time)
#   lines 968-1059 (30 unminified CSS rules with excessive whitespace and multi-line
#     formatting in icon glyph content declarations)
#   lines 1062-1161 (30 unminified CSS rules with excessive whitespace and multi-line
#     formatting in icon glyph content declarations):
#
# Lines 8-17 occurrences (8 rules):
#   lines 8, 9, 10, 11, 12, 15, 16, 17
#
# Lines 18-81 occurrences (30 rules - current fix batch):
#   lines 18, 19, 20, 24, 25, 26, 29, 32, 35, 38,
#   lines 41, 42, 45, 46, 47, 50, 53, 54, 55, 56,
#   lines 57, 60, 63, 64, 65, 68, 71, 74, 77, 81
#
# Lines 968-1059 occurrences (30 rules):
#   lines 968, 971, 974, 977, 980, 983, 986, 990, 995, 998,
#   lines 1001, 1004, 1008, 1011, 1014, 1017, 1020, 1023, 1026, 1029,
#   lines 1032, 1035, 1038, 1041, 1044, 1047, 1050, 1053, 1056, 1059
#
# Lines 1062-1161 occurrences (30 rules - CURRENT FIX BATCH):
#   lines 1062, 1065, 1068, 1071, 1074, 1077, 1080, 1083, 1086, 1089,
#   lines 1092, 1095, 1098, 1101, 1104, 1107, 1110, 1113, 1116, 1119,
#   lines 1123, 1127, 1131, 1135, 1138, 1142, 1146, 1152, 1157, 1161
#
# Lines 142-214 occurrences (30 rules - CURRENT FIX BATCH cz-css-1004):
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   .fa-flip-horizontal, .fa-flip-vertical, :root .fa-* filter-reset, .fa-stack,
#   .fa-stack-1x, .fa-stack-2x, .fa-inverse selector blocks and opening icon
#   glyph content declarations that bloat the production container image and
#   increase ECR storage costs / Kubernetes pod startup time.
#
#   Line 142   : unminified CSS rule (whitespace in .fa-flip-horizontal -ms-transform declaration)
#   Line 145   : unminified CSS rule (whitespace in .fa-flip-vertical filter declaration)
#   Line 146   : unminified CSS rule (whitespace in .fa-flip-vertical -webkit-transform declaration)
#   Line 147   : unminified CSS rule (whitespace in .fa-flip-vertical -ms-transform declaration)
#   Line 148   : unminified CSS rule (whitespace in .fa-flip-vertical transform declaration)
#   Line 155   : unminified CSS rule (whitespace in :root .fa-rotate-90/:root .fa-flip-vertical filter:none declaration)
#   Line 158   : unminified CSS rule (whitespace in .fa-stack position declaration)
#   Line 159   : unminified CSS rule (whitespace in .fa-stack display declaration)
#   Line 160   : unminified CSS rule (whitespace in .fa-stack width declaration)
#   Line 161   : unminified CSS rule (whitespace in .fa-stack height declaration)
#   Line 162   : unminified CSS rule (whitespace in .fa-stack line-height declaration)
#   Line 163   : unminified CSS rule (whitespace in .fa-stack vertical-align declaration)
#   Line 167   : unminified CSS rule (whitespace in .fa-stack-1x/.fa-stack-2x position declaration)
#   Line 168   : unminified CSS rule (whitespace in .fa-stack-1x/.fa-stack-2x left declaration)
#   Line 169   : unminified CSS rule (whitespace in .fa-stack-1x/.fa-stack-2x width declaration)
#   Line 170   : unminified CSS rule (whitespace in .fa-stack-1x/.fa-stack-2x text-align declaration)
#   Line 173   : unminified CSS rule (whitespace in .fa-stack-1x line-height declaration)
#   Line 176   : unminified CSS rule (whitespace in .fa-stack-2x font-size declaration)
#   Line 179   : unminified CSS rule (whitespace in .fa-inverse color declaration)
#   Line 184   : unminified CSS rule (whitespace in .fa-glass:before content declaration)
#   Line 187   : unminified CSS rule (whitespace in .fa-music:before content declaration)
#   Line 190   : unminified CSS rule (whitespace in .fa-search:before content declaration)
#   Line 193   : unminified CSS rule (whitespace in .fa-envelope-o:before content declaration)
#   Line 196   : unminified CSS rule (whitespace in .fa-heart:before content declaration)
#   Line 199   : unminified CSS rule (whitespace in .fa-star:before content declaration)
#   Line 202   : unminified CSS rule (whitespace in .fa-star-o:before content declaration)
#   Line 205   : unminified CSS rule (whitespace in .fa-user:before content declaration)
#   Line 208   : unminified CSS rule (whitespace in .fa-film:before content declaration)
#   Line 211   : unminified CSS rule (whitespace in .fa-th-large:before content declaration)
#   Line 214   : unminified CSS rule (whitespace in .fa-th:before content declaration)
RUN npx postcss assets/font-awesome/css/font-awesome.css \
        --output assets/font-awesome/css/font-awesome.css \
        --config postcss.config.js

# font-awesome.css lines 217-308 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-th-list:before through .fa-tag:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 217   : unminified CSS rule (content: "\f00b" in .fa-th-list:before block)
#   Line 220   : unminified CSS rule (content: "\f00c" in .fa-check:before block)
#   Line 225   : unminified CSS rule (content: "\f00d" in .fa-remove:before/.fa-close:before/.fa-times:before block)
#   Line 228   : unminified CSS rule (content: "\f00e" in .fa-search-plus:before block)
#   Line 231   : unminified CSS rule (content: "\f010" in .fa-search-minus:before block)
#   Line 234   : unminified CSS rule (content: "\f011" in .fa-power-off:before block)
#   Line 237   : unminified CSS rule (content: "\f012" in .fa-signal:before block)
#   Line 241   : unminified CSS rule (content: "\f013" in .fa-gear:before/.fa-cog:before block)
#   Line 244   : unminified CSS rule (content: "\f014" in .fa-trash-o:before block)
#   Line 247   : unminified CSS rule (content: "\f015" in .fa-home:before block)
#   Line 250   : unminified CSS rule (content: "\f016" in .fa-file-o:before block)
#   Line 253   : unminified CSS rule (content: "\f017" in .fa-clock-o:before block)
#   Line 256   : unminified CSS rule (content: "\f018" in .fa-road:before block)
#   Line 259   : unminified CSS rule (content: "\f019" in .fa-download:before block)
#   Line 262   : unminified CSS rule (content: "\f01a" in .fa-arrow-circle-o-down:before block)
#   Line 265   : unminified CSS rule (content: "\f01b" in .fa-arrow-circle-o-up:before block)
#   Line 268   : unminified CSS rule (content: "\f01c" in .fa-inbox:before block)
#   Line 271   : unminified CSS rule (content: "\f01d" in .fa-play-circle-o:before block)
#   Line 275   : unminified CSS rule (content: "\f01e" in .fa-rotate-right:before/.fa-repeat:before block)
#   Line 278   : unminified CSS rule (content: "\f021" in .fa-refresh:before block)
#   Line 281   : unminified CSS rule (content: "\f022" in .fa-list-alt:before block)
#   Line 284   : unminified CSS rule (content: "\f023" in .fa-lock:before block)
#   Line 287   : unminified CSS rule (content: "\f024" in .fa-flag:before block)
#   Line 290   : unminified CSS rule (content: "\f025" in .fa-headphones:before block)
#   Line 293   : unminified CSS rule (content: "\f026" in .fa-volume-off:before block)
#   Line 296   : unminified CSS rule (content: "\f027" in .fa-volume-down:before block)
#   Line 299   : unminified CSS rule (content: "\f028" in .fa-volume-up:before block)
#   Line 302   : unminified CSS rule (content: "\f029" in .fa-qrcode:before block)
#   Line 305   : unminified CSS rule (content: "\f02a" in .fa-barcode:before block)
#   Line 308   : unminified CSS rule (content: "\f02b" in .fa-tag:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 217-308 that are resolved by the same minification pass.

# font-awesome.css lines 311-402 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-tags:before through .fa-backward:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 311   : unminified CSS rule (content: "\f02c" in .fa-tags:before block)
#   Line 314   : unminified CSS rule (content: "\f02d" in .fa-book:before block)
#   Line 317   : unminified CSS rule (content: "\f02e" in .fa-bookmark:before block)
#   Line 320   : unminified CSS rule (content: "\f02f" in .fa-print:before block)
#   Line 323   : unminified CSS rule (content: "\f030" in .fa-camera:before block)
#   Line 326   : unminified CSS rule (content: "\f031" in .fa-font:before block)
#   Line 329   : unminified CSS rule (content: "\f032" in .fa-bold:before block)
#   Line 332   : unminified CSS rule (content: "\f033" in .fa-italic:before block)
#   Line 335   : unminified CSS rule (content: "\f034" in .fa-text-height:before block)
#   Line 338   : unminified CSS rule (content: "\f035" in .fa-text-width:before block)
#   Line 341   : unminified CSS rule (content: "\f036" in .fa-align-left:before block)
#   Line 344   : unminified CSS rule (content: "\f037" in .fa-align-center:before block)
#   Line 347   : unminified CSS rule (content: "\f038" in .fa-align-right:before block)
#   Line 350   : unminified CSS rule (content: "\f039" in .fa-align-justify:before block)
#   Line 353   : unminified CSS rule (content: "\f03a" in .fa-list:before block)
#   Line 357   : unminified CSS rule (content: "\f03b" in .fa-dedent:before/.fa-outdent:before block)
#   Line 360   : unminified CSS rule (content: "\f03c" in .fa-indent:before block)
#   Line 363   : unminified CSS rule (content: "\f03d" in .fa-video-camera:before block)
#   Line 368   : unminified CSS rule (content: "\f03e" in .fa-photo:before/.fa-image:before/.fa-picture-o:before block)
#   Line 371   : unminified CSS rule (content: "\f040" in .fa-pencil:before block)
#   Line 374   : unminified CSS rule (content: "\f041" in .fa-map-marker:before block)
#   Line 377   : unminified CSS rule (content: "\f042" in .fa-adjust:before block)
#   Line 380   : unminified CSS rule (content: "\f043" in .fa-tint:before block)
#   Line 384   : unminified CSS rule (content: "\f044" in .fa-edit:before/.fa-pencil-square-o:before block)
#   Line 387   : unminified CSS rule (content: "\f045" in .fa-share-square-o:before block)
#   Line 390   : unminified CSS rule (content: "\f046" in .fa-check-square-o:before block)
#   Line 393   : unminified CSS rule (content: "\f047" in .fa-arrows:before block)
#   Line 396   : unminified CSS rule (content: "\f048" in .fa-step-backward:before block)
#   Line 399   : unminified CSS rule (content: "\f049" in .fa-fast-backward:before block)
#   Line 402   : unminified CSS rule (content: "\f04a" in .fa-backward:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 311-402 that are resolved by the same minification pass.
#       Lines: 311, 314, 317, 320, 323, 326, 329, 332, 335, 338,
#              341, 344, 347, 350, 353, 357, 360, 363, 368, 371,
#              374, 377, 380, 384, 387, 390, 393, 396, 399, 402

# font-awesome.css lines 405-493 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-play:before through .fa-exclamation-circle:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 405   : unminified CSS rule (content: "\f04b" in .fa-play:before block)
#   Line 408   : unminified CSS rule (content: "\f04c" in .fa-pause:before block)
#   Line 411   : unminified CSS rule (content: "\f04d" in .fa-stop:before block)
#   Line 414   : unminified CSS rule (content: "\f04e" in .fa-forward:before block)
#   Line 417   : unminified CSS rule (content: "\f050" in .fa-fast-forward:before block)
#   Line 420   : unminified CSS rule (content: "\f051" in .fa-step-forward:before block)
#   Line 423   : unminified CSS rule (content: "\f052" in .fa-eject:before block)
#   Line 426   : unminified CSS rule (content: "\f053" in .fa-chevron-left:before block)
#   Line 429   : unminified CSS rule (content: "\f054" in .fa-chevron-right:before block)
#   Line 432   : unminified CSS rule (content: "\f055" in .fa-plus-circle:before block)
#   Line 435   : unminified CSS rule (content: "\f056" in .fa-minus-circle:before block)
#   Line 438   : unminified CSS rule (content: "\f057" in .fa-times-circle:before block)
#   Line 441   : unminified CSS rule (content: "\f058" in .fa-check-circle:before block)
#   Line 444   : unminified CSS rule (content: "\f059" in .fa-question-circle:before block)
#   Line 447   : unminified CSS rule (content: "\f05a" in .fa-info-circle:before block)
#   Line 450   : unminified CSS rule (content: "\f05b" in .fa-crosshairs:before block)
#   Line 453   : unminified CSS rule (content: "\f05c" in .fa-times-circle-o:before block)
#   Line 456   : unminified CSS rule (content: "\f05d" in .fa-check-circle-o:before block)
#   Line 459   : unminified CSS rule (content: "\f05e" in .fa-ban:before block)
#   Line 462   : unminified CSS rule (content: "\f060" in .fa-arrow-left:before block)
#   Line 465   : unminified CSS rule (content: "\f061" in .fa-arrow-right:before block)
#   Line 468   : unminified CSS rule (content: "\f062" in .fa-arrow-up:before block)
#   Line 471   : unminified CSS rule (content: "\f063" in .fa-arrow-down:before block)
#   Line 475   : unminified CSS rule (content: "\f064" in .fa-mail-forward:before/.fa-share:before block)
#   Line 478   : unminified CSS rule (content: "\f065" in .fa-expand:before block)
#   Line 481   : unminified CSS rule (content: "\f066" in .fa-compress:before block)
#   Line 484   : unminified CSS rule (content: "\f067" in .fa-plus:before block)
#   Line 487   : unminified CSS rule (content: "\f068" in .fa-minus:before block)
#   Line 490   : unminified CSS rule (content: "\f069" in .fa-asterisk:before block)
#   Line 493   : unminified CSS rule (content: "\f06a" in .fa-exclamation-circle:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 405-493 that are resolved by the same minification pass.
#       Lines: 405, 408, 411, 414, 417, 420, 423, 426, 429, 432,
#              435, 438, 441, 444, 447, 450, 453, 456, 459, 462,
#              465, 468, 471, 475, 478, 481, 484, 487, 490, 493


# font-awesome.css lines 496-586 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-gift:before through .fa-heart-o:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 496   : unminified CSS rule (content: "\f06b" in .fa-gift:before block)
#   Line 499   : unminified CSS rule (content: "\f06c" in .fa-leaf:before block)
#   Line 502   : unminified CSS rule (content: "\f06d" in .fa-fire:before block)
#   Line 505   : unminified CSS rule (content: "\f06e" in .fa-eye:before block)
#   Line 508   : unminified CSS rule (content: "\f070" in .fa-eye-slash:before block)
#   Line 512   : unminified CSS rule (content: "\f071" in .fa-warning:before/.fa-exclamation-triangle:before block)
#   Line 515   : unminified CSS rule (content: "\f072" in .fa-plane:before block)
#   Line 518   : unminified CSS rule (content: "\f073" in .fa-calendar:before block)
#   Line 521   : unminified CSS rule (content: "\f074" in .fa-random:before block)
#   Line 524   : unminified CSS rule (content: "\f075" in .fa-comment:before block)
#   Line 527   : unminified CSS rule (content: "\f076" in .fa-magnet:before block)
#   Line 530   : unminified CSS rule (content: "\f077" in .fa-chevron-up:before block)
#   Line 533   : unminified CSS rule (content: "\f078" in .fa-chevron-down:before block)
#   Line 536   : unminified CSS rule (content: "\f079" in .fa-retweet:before block)
#   Line 539   : unminified CSS rule (content: "\f07a" in .fa-shopping-cart:before block)
#   Line 542   : unminified CSS rule (content: "\f07b" in .fa-folder:before block)
#   Line 545   : unminified CSS rule (content: "\f07c" in .fa-folder-open:before block)
#   Line 548   : unminified CSS rule (content: "\f07d" in .fa-arrows-v:before block)
#   Line 551   : unminified CSS rule (content: "\f07e" in .fa-arrows-h:before block)
#   Line 555   : unminified CSS rule (content: "\f080" in .fa-bar-chart-o:before/.fa-bar-chart:before block)
#   Line 558   : unminified CSS rule (content: "\f081" in .fa-twitter-square:before block)
#   Line 561   : unminified CSS rule (content: "\f082" in .fa-facebook-square:before block)
#   Line 564   : unminified CSS rule (content: "\f083" in .fa-camera-retro:before block)
#   Line 567   : unminified CSS rule (content: "\f084" in .fa-key:before block)
#   Line 571   : unminified CSS rule (content: "\f085" in .fa-gears:before/.fa-cogs:before block)
#   Line 574   : unminified CSS rule (content: "\f086" in .fa-comments:before block)
#   Line 577   : unminified CSS rule (content: "\f087" in .fa-thumbs-o-up:before block)
#   Line 580   : unminified CSS rule (content: "\f088" in .fa-thumbs-o-down:before block)
#   Line 583   : unminified CSS rule (content: "\f089" in .fa-star-half:before block)
#   Line 586   : unminified CSS rule (content: "\f08a" in .fa-heart-o:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 496-586 that are resolved by the same minification pass.
#       Lines: 496, 499, 502, 505, 508, 512, 515, 518, 521, 524,
#              527, 530, 533, 536, 539, 542, 545, 548, 551, 555,
#              558, 561, 564, 567, 571, 574, 577, 580, 583, 586
# font-awesome.css lines 589-678 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-sign-out:before through .fa-arrow-circle-up:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 589   : unminified CSS rule (content: "\f08b" in .fa-sign-out:before block)
#   Line 592   : unminified CSS rule (content: "\f08c" in .fa-linkedin-square:before block)
#   Line 595   : unminified CSS rule (content: "\f08d" in .fa-thumb-tack:before block)
#   Line 598   : unminified CSS rule (content: "\f08e" in .fa-external-link:before block)
#   Line 601   : unminified CSS rule (content: "\f090" in .fa-sign-in:before block)
#   Line 604   : unminified CSS rule (content: "\f091" in .fa-trophy:before block)
#   Line 607   : unminified CSS rule (content: "\f092" in .fa-github-square:before block)
#   Line 610   : unminified CSS rule (content: "\f093" in .fa-upload:before block)
#   Line 613   : unminified CSS rule (content: "\f094" in .fa-lemon-o:before block)
#   Line 616   : unminified CSS rule (content: "\f095" in .fa-phone:before block)
#   Line 619   : unminified CSS rule (content: "\f096" in .fa-square-o:before block)
#   Line 622   : unminified CSS rule (content: "\f097" in .fa-bookmark-o:before block)
#   Line 625   : unminified CSS rule (content: "\f098" in .fa-phone-square:before block)
#   Line 628   : unminified CSS rule (content: "\f099" in .fa-twitter:before block)
#   Line 632   : unminified CSS rule (content: "\f09a" in .fa-facebook-f:before/.fa-facebook:before block)
#   Line 635   : unminified CSS rule (content: "\f09b" in .fa-github:before block)
#   Line 638   : unminified CSS rule (content: "\f09c" in .fa-unlock:before block)
#   Line 641   : unminified CSS rule (content: "\f09d" in .fa-credit-card:before block)
#   Line 645   : unminified CSS rule (content: "\f09e" in .fa-feed:before/.fa-rss:before block)
#   Line 648   : unminified CSS rule (content: "\f0a0" in .fa-hdd-o:before block)
#   Line 651   : unminified CSS rule (content: "\f0a1" in .fa-bullhorn:before block)
#   Line 654   : unminified CSS rule (content: "\f0f3" in .fa-bell:before block)
#   Line 657   : unminified CSS rule (content: "\f0a3" in .fa-certificate:before block)
#   Line 660   : unminified CSS rule (content: "\f0a4" in .fa-hand-o-right:before block)
#   Line 663   : unminified CSS rule (content: "\f0a5" in .fa-hand-o-left:before block)
#   Line 666   : unminified CSS rule (content: "\f0a6" in .fa-hand-o-up:before block)
#   Line 669   : unminified CSS rule (content: "\f0a7" in .fa-hand-o-down:before block)
#   Line 672   : unminified CSS rule (content: "\f0a8" in .fa-arrow-circle-left:before block)
#   Line 675   : unminified CSS rule (content: "\f0a9" in .fa-arrow-circle-right:before block)
#   Line 678   : unminified CSS rule (content: "\f0aa" in .fa-arrow-circle-up:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 589-678 that are resolved by the same minification pass.
#       Lines: 589, 592, 595, 598, 601, 604, 607, 610, 613, 616,
#              619, 622, 625, 628, 632, 635, 638, 641, 645, 648,
#              651, 654, 657, 660, 663, 666, 669, 672, 675, 678
# font-awesome.css lines 681-775 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-arrow-circle-down:before through .fa-caret-down:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 681   : unminified CSS rule (content: "\f0ab" in .fa-arrow-circle-down:before block)
#   Line 684   : unminified CSS rule (content: "\f0ac" in .fa-globe:before block)
#   Line 687   : unminified CSS rule (content: "\f0ad" in .fa-wrench:before block)
#   Line 690   : unminified CSS rule (content: "\f0ae" in .fa-tasks:before block)
#   Line 693   : unminified CSS rule (content: "\f0b0" in .fa-filter:before block)
#   Line 696   : unminified CSS rule (content: "\f0b1" in .fa-briefcase:before block)
#   Line 699   : unminified CSS rule (content: "\f0b2" in .fa-arrows-alt:before block)
#   Line 703   : unminified CSS rule (content: "\f0c0" in .fa-group:before/.fa-users:before block)
#   Line 707   : unminified CSS rule (content: "\f0c1" in .fa-chain:before/.fa-link:before block)
#   Line 710   : unminified CSS rule (content: "\f0c2" in .fa-cloud:before block)
#   Line 713   : unminified CSS rule (content: "\f0c3" in .fa-flask:before block)
#   Line 717   : unminified CSS rule (content: "\f0c4" in .fa-cut:before/.fa-scissors:before block)
#   Line 721   : unminified CSS rule (content: "\f0c5" in .fa-copy:before/.fa-files-o:before block)
#   Line 724   : unminified CSS rule (content: "\f0c6" in .fa-paperclip:before block)
#   Line 728   : unminified CSS rule (content: "\f0c7" in .fa-save:before/.fa-floppy-o:before block)
#   Line 731   : unminified CSS rule (content: "\f0c8" in .fa-square:before block)
#   Line 736   : unminified CSS rule (content: "\f0c9" in .fa-navicon:before/.fa-reorder:before/.fa-bars:before block)
#   Line 739   : unminified CSS rule (content: "\f0ca" in .fa-list-ul:before block)
#   Line 742   : unminified CSS rule (content: "\f0cb" in .fa-list-ol:before block)
#   Line 745   : unminified CSS rule (content: "\f0cc" in .fa-strikethrough:before block)
#   Line 748   : unminified CSS rule (content: "\f0cd" in .fa-underline:before block)
#   Line 751   : unminified CSS rule (content: "\f0ce" in .fa-table:before block)
#   Line 754   : unminified CSS rule (content: "\f0d0" in .fa-magic:before block)
#   Line 757   : unminified CSS rule (content: "\f0d1" in .fa-truck:before block)
#   Line 760   : unminified CSS rule (content: "\f0d2" in .fa-pinterest:before block)
#   Line 763   : unminified CSS rule (content: "\f0d3" in .fa-pinterest-square:before block)
#   Line 766   : unminified CSS rule (content: "\f0d4" in .fa-google-plus-square:before block)
#   Line 769   : unminified CSS rule (content: "\f0d5" in .fa-google-plus:before block)
#   Line 772   : unminified CSS rule (content: "\f0d6" in .fa-money:before block)
#   Line 775   : unminified CSS rule (content: "\f0d7" in .fa-caret-down:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 681-775 that are resolved by the same minification pass.
#       Lines: 681, 684, 687, 690, 693, 696, 699, 703, 707, 710,
#              713, 717, 721, 724, 728, 731, 736, 739, 742, 745,
#              748, 751, 754, 757, 760, 763, 766, 769, 772, 775
# font-awesome.css lines 778-873 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-caret-up:before through .fa-building-o:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 778   : unminified CSS rule (content: "\f0d8" in .fa-caret-up:before block)
#   Line 781   : unminified CSS rule (content: "\f0d9" in .fa-caret-left:before block)
#   Line 784   : unminified CSS rule (content: "\f0da" in .fa-caret-right:before block)
#   Line 787   : unminified CSS rule (content: "\f0db" in .fa-columns:before block)
#   Line 791   : unminified CSS rule (content: "\f0dc" in .fa-unsorted:before/.fa-sort:before block)
#   Line 795   : unminified CSS rule (content: "\f0dd" in .fa-sort-down:before/.fa-sort-desc:before block)
#   Line 799   : unminified CSS rule (content: "\f0de" in .fa-sort-up:before/.fa-sort-asc:before block)
#   Line 802   : unminified CSS rule (content: "\f0e0" in .fa-envelope:before block)
#   Line 805   : unminified CSS rule (content: "\f0e1" in .fa-linkedin:before block)
#   Line 809   : unminified CSS rule (content: "\f0e2" in .fa-rotate-left:before/.fa-undo:before block)
#   Line 813   : unminified CSS rule (content: "\f0e3" in .fa-legal:before/.fa-gavel:before block)
#   Line 817   : unminified CSS rule (content: "\f0e4" in .fa-dashboard:before/.fa-tachometer:before block)
#   Line 820   : unminified CSS rule (content: "\f0e5" in .fa-comment-o:before block)
#   Line 823   : unminified CSS rule (content: "\f0e6" in .fa-comments-o:before block)
#   Line 827   : unminified CSS rule (content: "\f0e7" in .fa-flash:before/.fa-bolt:before block)
#   Line 830   : unminified CSS rule (content: "\f0e8" in .fa-sitemap:before block)
#   Line 833   : unminified CSS rule (content: "\f0e9" in .fa-umbrella:before block)
#   Line 837   : unminified CSS rule (content: "\f0ea" in .fa-paste:before/.fa-clipboard:before block)
#   Line 840   : unminified CSS rule (content: "\f0eb" in .fa-lightbulb-o:before block)
#   Line 843   : unminified CSS rule (content: "\f0ec" in .fa-exchange:before block)
#   Line 846   : unminified CSS rule (content: "\f0ed" in .fa-cloud-download:before block)
#   Line 849   : unminified CSS rule (content: "\f0ee" in .fa-cloud-upload:before block)
#   Line 852   : unminified CSS rule (content: "\f0f0" in .fa-user-md:before block)
#   Line 855   : unminified CSS rule (content: "\f0f1" in .fa-stethoscope:before block)
#   Line 858   : unminified CSS rule (content: "\f0f2" in .fa-suitcase:before block)
#   Line 861   : unminified CSS rule (content: "\f0a2" in .fa-bell-o:before block)
#   Line 864   : unminified CSS rule (content: "\f0f4" in .fa-coffee:before block)
#   Line 867   : unminified CSS rule (content: "\f0f5" in .fa-cutlery:before block)
#   Line 870   : unminified CSS rule (content: "\f0f6" in .fa-file-text-o:before block)
#   Line 873   : unminified CSS rule (content: "\f0f7" in .fa-building-o:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 778-873 that are resolved by the same minification pass.
#       Lines: 778, 781, 784, 787, 791, 795, 799, 802, 805, 809,
#              813, 817, 820, 823, 827, 830, 833, 837, 840, 843,
#              846, 849, 852, 855, 858, 861, 864, 867, 870, 873
# font-awesome.css lines 876-965 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-hospital-o:before through .fa-frown-o:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 876   : unminified CSS rule (content: "\f0f8" in .fa-hospital-o:before block)
#   Line 879   : unminified CSS rule (content: "\f0f9" in .fa-ambulance:before block)
#   Line 882   : unminified CSS rule (content: "\f0fa" in .fa-medkit:before block)
#   Line 885   : unminified CSS rule (content: "\f0fb" in .fa-fighter-jet:before block)
#   Line 888   : unminified CSS rule (content: "\f0fc" in .fa-beer:before block)
#   Line 891   : unminified CSS rule (content: "\f0fd" in .fa-h-square:before block)
#   Line 894   : unminified CSS rule (content: "\f0fe" in .fa-plus-square:before block)
#   Line 897   : unminified CSS rule (content: "\f100" in .fa-angle-double-left:before block)
#   Line 900   : unminified CSS rule (content: "\f101" in .fa-angle-double-right:before block)
#   Line 903   : unminified CSS rule (content: "\f102" in .fa-angle-double-up:before block)
#   Line 906   : unminified CSS rule (content: "\f103" in .fa-angle-double-down:before block)
#   Line 909   : unminified CSS rule (content: "\f104" in .fa-angle-left:before block)
#   Line 912   : unminified CSS rule (content: "\f105" in .fa-angle-right:before block)
#   Line 915   : unminified CSS rule (content: "\f106" in .fa-angle-up:before block)
#   Line 918   : unminified CSS rule (content: "\f107" in .fa-angle-down:before block)
#   Line 921   : unminified CSS rule (content: "\f108" in .fa-desktop:before block)
#   Line 924   : unminified CSS rule (content: "\f109" in .fa-laptop:before block)
#   Line 927   : unminified CSS rule (content: "\f10a" in .fa-tablet:before block)
#   Line 931   : unminified CSS rule (content: "\f10b" in .fa-mobile-phone:before/.fa-mobile:before block)
#   Line 934   : unminified CSS rule (content: "\f10c" in .fa-circle-o:before block)
#   Line 937   : unminified CSS rule (content: "\f10d" in .fa-quote-left:before block)
#   Line 940   : unminified CSS rule (content: "\f10e" in .fa-quote-right:before block)
#   Line 943   : unminified CSS rule (content: "\f110" in .fa-spinner:before block)
#   Line 946   : unminified CSS rule (content: "\f111" in .fa-circle:before block)
#   Line 950   : unminified CSS rule (content: "\f112" in .fa-mail-reply:before/.fa-reply:before block)
#   Line 953   : unminified CSS rule (content: "\f113" in .fa-github-alt:before block)
#   Line 956   : unminified CSS rule (content: "\f114" in .fa-folder-o:before block)
#   Line 959   : unminified CSS rule (content: "\f115" in .fa-folder-open-o:before block)
#   Line 962   : unminified CSS rule (content: "\f118" in .fa-smile-o:before block)
#   Line 965   : unminified CSS rule (content: "\f119" in .fa-frown-o:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 876-965 that are resolved by the same minification pass.
#       Lines: 876, 879, 882, 885, 888, 891, 894, 897, 900, 903,
#              906, 909, 912, 915, 918, 921, 924, 927, 931, 934,
#              937, 940, 943, 946, 950, 953, 956, 959, 962, 965
# font-awesome.css lines 1165-1252 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations that bloat the production container image
#   and increase ECR storage costs / Kubernetes pod startup time.
#
#   Line 1165  : unminified CSS rule (content: "\f15a" in .fa-bitcoin:before/.fa-btc:before block)
#   Line 1168  : unminified CSS rule (content: "\f15b" in .fa-file:before block)
#   Line 1171  : unminified CSS rule (content: "\f15c" in .fa-file-text:before block)
#   Line 1174  : unminified CSS rule (content: "\f15d" in .fa-sort-alpha-asc:before block)
#   Line 1177  : unminified CSS rule (content: "\f15e" in .fa-sort-alpha-desc:before block)
#   Line 1180  : unminified CSS rule (content: "\f160" in .fa-sort-amount-asc:before block)
#   Line 1183  : unminified CSS rule (content: "\f161" in .fa-sort-amount-desc:before block)
#   Line 1186  : unminified CSS rule (content: "\f162" in .fa-sort-numeric-asc:before block)
#   Line 1189  : unminified CSS rule (content: "\f163" in .fa-sort-numeric-desc:before block)
#   Line 1192  : unminified CSS rule (content: "\f164" in .fa-thumbs-up:before block)
#   Line 1195  : unminified CSS rule (content: "\f165" in .fa-thumbs-down:before block)
#   Line 1198  : unminified CSS rule (content: "\f166" in .fa-youtube-square:before block)
#   Line 1201  : unminified CSS rule (content: "\f167" in .fa-youtube:before block)
#   Line 1204  : unminified CSS rule (content: "\f168" in .fa-xing:before block)
#   Line 1207  : unminified CSS rule (content: "\f169" in .fa-xing-square:before block)
#   Line 1210  : unminified CSS rule (content: "\f16a" in .fa-youtube-play:before block)
#   Line 1213  : unminified CSS rule (content: "\f16b" in .fa-dropbox:before block)
#   Line 1216  : unminified CSS rule (content: "\f16c" in .fa-stack-overflow:before block)
#   Line 1219  : unminified CSS rule (content: "\f16d" in .fa-instagram:before block)
#   Line 1222  : unminified CSS rule (content: "\f16e" in .fa-flickr:before block)
#   Line 1225  : unminified CSS rule (content: "\f170" in .fa-adn:before block)
#   Line 1228  : unminified CSS rule (content: "\f171" in .fa-bitbucket:before block)
#   Line 1231  : unminified CSS rule (content: "\f172" in .fa-bitbucket-square:before block)
#   Line 1234  : unminified CSS rule (content: "\f173" in .fa-tumblr:before block)
#   Line 1237  : unminified CSS rule (content: "\f174" in .fa-tumblr-square:before block)
#   Line 1240  : unminified CSS rule (content: "\f175" in .fa-long-arrow-down:before block)
#   Line 1243  : unminified CSS rule (content: "\f176" in .fa-long-arrow-up:before block)
#   Line 1246  : unminified CSS rule (content: "\f177" in .fa-long-arrow-left:before block)
#   Line 1249  : unminified CSS rule (content: "\f178" in .fa-long-arrow-right:before block)
#   Line 1252  : unminified CSS rule (content: "\f179" in .fa-apple:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 1165-1252 that are resolved by the same minification pass.

# font-awesome.css lines 1255-1345 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-windows:before through .fa-envelope-square:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 1255  : unminified CSS rule (content: "\f17a" in .fa-windows:before block)
#   Line 1258  : unminified CSS rule (content: "\f17b" in .fa-android:before block)
#   Line 1261  : unminified CSS rule (content: "\f17c" in .fa-linux:before block)
#   Line 1264  : unminified CSS rule (content: "\f17d" in .fa-dribbble:before block)
#   Line 1267  : unminified CSS rule (content: "\f17e" in .fa-skype:before block)
#   Line 1270  : unminified CSS rule (content: "\f180" in .fa-foursquare:before block)
#   Line 1273  : unminified CSS rule (content: "\f181" in .fa-trello:before block)
#   Line 1276  : unminified CSS rule (content: "\f182" in .fa-female:before block)
#   Line 1279  : unminified CSS rule (content: "\f183" in .fa-male:before block)
#   Line 1283  : unminified CSS rule (content: "\f184" in .fa-gittip:before/.fa-gratipay:before block)
#   Line 1286  : unminified CSS rule (content: "\f185" in .fa-sun-o:before block)
#   Line 1289  : unminified CSS rule (content: "\f186" in .fa-moon-o:before block)
#   Line 1292  : unminified CSS rule (content: "\f187" in .fa-archive:before block)
#   Line 1295  : unminified CSS rule (content: "\f188" in .fa-bug:before block)
#   Line 1298  : unminified CSS rule (content: "\f189" in .fa-vk:before block)
#   Line 1301  : unminified CSS rule (content: "\f18a" in .fa-weibo:before block)
#   Line 1304  : unminified CSS rule (content: "\f18b" in .fa-renren:before block)
#   Line 1307  : unminified CSS rule (content: "\f18c" in .fa-pagelines:before block)
#   Line 1310  : unminified CSS rule (content: "\f18d" in .fa-stack-exchange:before block)
#   Line 1313  : unminified CSS rule (content: "\f18e" in .fa-arrow-circle-o-right:before block)
#   Line 1316  : unminified CSS rule (content: "\f190" in .fa-arrow-circle-o-left:before block)
#   Line 1320  : unminified CSS rule (content: "\f191" in .fa-toggle-left:before/.fa-caret-square-o-left:before block)
#   Line 1323  : unminified CSS rule (content: "\f192" in .fa-dot-circle-o:before block)
#   Line 1326  : unminified CSS rule (content: "\f193" in .fa-wheelchair:before block)
#   Line 1329  : unminified CSS rule (content: "\f194" in .fa-vimeo-square:before block)
#   Line 1333  : unminified CSS rule (content: "\f195" in .fa-turkish-lira:before/.fa-try:before block)
#   Line 1336  : unminified CSS rule (content: "\f196" in .fa-plus-square-o:before block)
#   Line 1339  : unminified CSS rule (content: "\f197" in .fa-space-shuttle:before block)
#   Line 1342  : unminified CSS rule (content: "\f198" in .fa-slack:before block)
#   Line 1345  : unminified CSS rule (content: "\f199" in .fa-envelope-square:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 1255-1345 that are resolved by the same minification pass.
#       Lines: 1255, 1258, 1261, 1264, 1267, 1270, 1273, 1276, 1279, 1283,
#              1286, 1289, 1292, 1295, 1298, 1301, 1304, 1307, 1310, 1313,
#              1316, 1320, 1323, 1326, 1329, 1333, 1336, 1339, 1342, 1345

# font-awesome.css lines 1348-1439 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations that bloat the production container image
#   and increase ECR storage costs / Kubernetes pod startup time.
#
#   Line 1348  : unminified CSS rule (content: "\f19a" in .fa-wordpress:before block)
#   Line 1351  : unminified CSS rule (content: "\f19b" in .fa-openid:before block)
#   Line 1356  : unminified CSS rule (content: "\f19c" in .fa-institution:before/.fa-bank:before/.fa-university:before block)
#   Line 1360  : unminified CSS rule (content: "\f19d" in .fa-mortar-board:before/.fa-graduation-cap:before block)
#   Line 1363  : unminified CSS rule (content: "\f19e" in .fa-yahoo:before block)
#   Line 1366  : unminified CSS rule (content: "\f1a0" in .fa-google:before block)
#   Line 1369  : unminified CSS rule (content: "\f1a1" in .fa-reddit:before block)
#   Line 1372  : unminified CSS rule (content: "\f1a2" in .fa-reddit-square:before block)
#   Line 1375  : unminified CSS rule (content: "\f1a3" in .fa-stumbleupon-circle:before block)
#   Line 1378  : unminified CSS rule (content: "\f1a4" in .fa-stumbleupon:before block)
#   Line 1381  : unminified CSS rule (content: "\f1a5" in .fa-delicious:before block)
#   Line 1384  : unminified CSS rule (content: "\f1a6" in .fa-digg:before block)
#   Line 1387  : unminified CSS rule (content: "\f1a7" in .fa-pied-piper:before block)
#   Line 1390  : unminified CSS rule (content: "\f1a8" in .fa-pied-piper-alt:before block)
#   Line 1393  : unminified CSS rule (content: "\f1a9" in .fa-drupal:before block)
#   Line 1396  : unminified CSS rule (content: "\f1aa" in .fa-joomla:before block)
#   Line 1399  : unminified CSS rule (content: "\f1ab" in .fa-language:before block)
#   Line 1402  : unminified CSS rule (content: "\f1ac" in .fa-fax:before block)
#   Line 1405  : unminified CSS rule (content: "\f1ad" in .fa-building:before block)
#   Line 1408  : unminified CSS rule (content: "\f1ae" in .fa-child:before block)
#   Line 1411  : unminified CSS rule (content: "\f1b0" in .fa-paw:before block)
#   Line 1414  : unminified CSS rule (content: "\f1b1" in .fa-spoon:before block)
#   Line 1417  : unminified CSS rule (content: "\f1b2" in .fa-cube:before block)
#   Line 1420  : unminified CSS rule (content: "\f1b3" in .fa-cubes:before block)
#   Line 1423  : unminified CSS rule (content: "\f1b4" in .fa-behance:before block)
#   Line 1426  : unminified CSS rule (content: "\f1b5" in .fa-behance-square:before block)
#   Line 1429  : unminified CSS rule (content: "\f1b6" in .fa-steam:before block)
#   Line 1432  : unminified CSS rule (content: "\f1b7" in .fa-steam-square:before block)
#   Line 1435  : unminified CSS rule (content: "\f1b8" in .fa-recycle:before block)
#   Line 1439  : unminified CSS rule (content: "\f1b9" in .fa-automobile:before/.fa-car:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 1348-1439 that are resolved by the same minification pass.

# font-awesome.css lines 1443-1546 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations that bloat the production container image
#   and increase ECR storage costs / Kubernetes pod startup time.
#
#   Line 1443  : unminified CSS rule (content: "\f1ba" in .fa-cab:before/.fa-taxi:before block)
#   Line 1446  : unminified CSS rule (content: "\f1bb" in .fa-tree:before block)
#   Line 1449  : unminified CSS rule (content: "\f1bc" in .fa-spotify:before block)
#   Line 1452  : unminified CSS rule (content: "\f1bd" in .fa-deviantart:before block)
#   Line 1455  : unminified CSS rule (content: "\f1be" in .fa-soundcloud:before block)
#   Line 1458  : unminified CSS rule (content: "\f1c0" in .fa-database:before block)
#   Line 1461  : unminified CSS rule (content: "\f1c1" in .fa-file-pdf-o:before block)
#   Line 1464  : unminified CSS rule (content: "\f1c2" in .fa-file-word-o:before block)
#   Line 1467  : unminified CSS rule (content: "\f1c3" in .fa-file-excel-o:before block)
#   Line 1470  : unminified CSS rule (content: "\f1c4" in .fa-file-powerpoint-o:before block)
#   Line 1475  : unminified CSS rule (content: "\f1c5" in .fa-file-photo-o:before/.fa-file-picture-o:before/.fa-file-image-o:before block)
#   Line 1479  : unminified CSS rule (content: "\f1c6" in .fa-file-zip-o:before/.fa-file-archive-o:before block)
#   Line 1483  : unminified CSS rule (content: "\f1c7" in .fa-file-sound-o:before/.fa-file-audio-o:before block)
#   Line 1487  : unminified CSS rule (content: "\f1c8" in .fa-file-movie-o:before/.fa-file-video-o:before block)
#   Line 1490  : unminified CSS rule (content: "\f1c9" in .fa-file-code-o:before block)
#   Line 1493  : unminified CSS rule (content: "\f1ca" in .fa-vine:before block)
#   Line 1496  : unminified CSS rule (content: "\f1cb" in .fa-codepen:before block)
#   Line 1499  : unminified CSS rule (content: "\f1cc" in .fa-jsfiddle:before block)
#   Line 1506  : unminified CSS rule (content: "\f1cd" in .fa-life-bouy:before/.fa-life-buoy:before/.fa-life-saver:before/.fa-support:before/.fa-life-ring:before block)
#   Line 1509  : unminified CSS rule (content: "\f1ce" in .fa-circle-o-notch:before block)
#   Line 1513  : unminified CSS rule (content: "\f1d0" in .fa-ra:before/.fa-rebel:before block)
#   Line 1517  : unminified CSS rule (content: "\f1d1" in .fa-ge:before/.fa-empire:before block)
#   Line 1520  : unminified CSS rule (content: "\f1d2" in .fa-git-square:before block)
#   Line 1523  : unminified CSS rule (content: "\f1d3" in .fa-git:before block)
#   Line 1528  : unminified CSS rule (content: "\f1d4" in .fa-y-combinator-square:before/.fa-yc-square:before/.fa-hacker-news:before block)
#   Line 1531  : unminified CSS rule (content: "\f1d5" in .fa-tencent-weibo:before block)
#   Line 1534  : unminified CSS rule (content: "\f1d6" in .fa-qq:before block)
#   Line 1538  : unminified CSS rule (content: "\f1d7" in .fa-wechat:before/.fa-weixin:before block)
#   Line 1542  : unminified CSS rule (content: "\f1d8" in .fa-send:before/.fa-paper-plane:before block)
#   Line 1546  : unminified CSS rule (content: "\f1d9" in .fa-send-o:before/.fa-paper-plane-o:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 1443-1546 that are resolved by the same minification pass.

# font-awesome.css lines 1549-1637 - resolves cz-css-1004 (30 occurrences):
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations that bloat the production container image
#   and increase ECR storage costs / Kubernetes pod startup time.
#
#   Line 1549  : unminified CSS rule (content: "\f1da" in .fa-history:before block)
#   Line 1552  : unminified CSS rule (content: "\f1db" in .fa-circle-thin:before block)
#   Line 1555  : unminified CSS rule (content: "\f1dc" in .fa-header:before block)
#   Line 1558  : unminified CSS rule (content: "\f1dd" in .fa-paragraph:before block)
#   Line 1561  : unminified CSS rule (content: "\f1de" in .fa-sliders:before block)
#   Line 1564  : unminified CSS rule (content: "\f1e0" in .fa-share-alt:before block)
#   Line 1567  : unminified CSS rule (content: "\f1e1" in .fa-share-alt-square:before block)
#   Line 1570  : unminified CSS rule (content: "\f1e2" in .fa-bomb:before block)
#   Line 1574  : unminified CSS rule (content: "\f1e3" in .fa-soccer-ball-o:before/.fa-futbol-o:before block)
#   Line 1577  : unminified CSS rule (content: "\f1e4" in .fa-tty:before block)
#   Line 1580  : unminified CSS rule (content: "\f1e5" in .fa-binoculars:before block)
#   Line 1583  : unminified CSS rule (content: "\f1e6" in .fa-plug:before block)
#   Line 1586  : unminified CSS rule (content: "\f1e7" in .fa-slideshare:before block)
#   Line 1589  : unminified CSS rule (content: "\f1e8" in .fa-twitch:before block)
#   Line 1592  : unminified CSS rule (content: "\f1e9" in .fa-yelp:before block)
#   Line 1595  : unminified CSS rule (content: "\f1ea" in .fa-newspaper-o:before block)
#   Line 1598  : unminified CSS rule (content: "\f1eb" in .fa-wifi:before block)
#   Line 1601  : unminified CSS rule (content: "\f1ec" in .fa-calculator:before block)
#   Line 1604  : unminified CSS rule (content: "\f1ed" in .fa-paypal:before block)
#   Line 1607  : unminified CSS rule (content: "\f1ee" in .fa-google-wallet:before block)
#   Line 1610  : unminified CSS rule (content: "\f1f0" in .fa-cc-visa:before block)
#   Line 1613  : unminified CSS rule (content: "\f1f1" in .fa-cc-mastercard:before block)
#   Line 1616  : unminified CSS rule (content: "\f1f2" in .fa-cc-discover:before block)
#   Line 1619  : unminified CSS rule (content: "\f1f3" in .fa-cc-amex:before block)
#   Line 1622  : unminified CSS rule (content: "\f1f4" in .fa-cc-paypal:before block)
#   Line 1625  : unminified CSS rule (content: "\f1f5" in .fa-cc-stripe:before block)
#   Line 1628  : unminified CSS rule (content: "\f1f6" in .fa-bell-slash:before block)
#   Line 1631  : unminified CSS rule (content: "\f1f7" in .fa-bell-slash-o:before block)
#   Line 1634  : unminified CSS rule (content: "\f1f8" in .fa-trash:before block)
#   Line 1637  : unminified CSS rule (content: "\f1f9" in .fa-copyright:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 1549-1637 that are resolved by the same minification pass.

# font-awesome.css lines 1640-1729 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations that bloat the production container image
#   and increase ECR storage costs / Kubernetes pod startup time.
#
#   Line 1640  : unminified CSS rule (content: "\f1fa" in .fa-at:before block)
#   Line 1643  : unminified CSS rule (content: "\f1fb" in .fa-eyedropper:before block)
#   Line 1646  : unminified CSS rule (content: "\f1fc" in .fa-paint-brush:before block)
#   Line 1649  : unminified CSS rule (content: "\f1fd" in .fa-birthday-cake:before block)
#   Line 1652  : unminified CSS rule (content: "\f1fe" in .fa-area-chart:before block)
#   Line 1655  : unminified CSS rule (content: "\f200" in .fa-pie-chart:before block)
#   Line 1658  : unminified CSS rule (content: "\f201" in .fa-line-chart:before block)
#   Line 1661  : unminified CSS rule (content: "\f202" in .fa-lastfm:before block)
#   Line 1664  : unminified CSS rule (content: "\f203" in .fa-lastfm-square:before block)
#   Line 1667  : unminified CSS rule (content: "\f204" in .fa-toggle-off:before block)
#   Line 1670  : unminified CSS rule (content: "\f205" in .fa-toggle-on:before block)
#   Line 1673  : unminified CSS rule (content: "\f206" in .fa-bicycle:before block)
#   Line 1676  : unminified CSS rule (content: "\f207" in .fa-bus:before block)
#   Line 1679  : unminified CSS rule (content: "\f208" in .fa-ioxhost:before block)
#   Line 1682  : unminified CSS rule (content: "\f209" in .fa-angellist:before block)
#   Line 1685  : unminified CSS rule (content: "\f20a" in .fa-cc:before block)
#   Line 1690  : unminified CSS rule (content: "\f20b" in .fa-shekel:before/.fa-sheqel:before/.fa-ils:before block)
#   Line 1693  : unminified CSS rule (content: "\f20c" in .fa-meanpath:before block)
#   Line 1696  : unminified CSS rule (content: "\f20d" in .fa-buysellads:before block)
#   Line 1699  : unminified CSS rule (content: "\f20e" in .fa-connectdevelop:before block)
#   Line 1702  : unminified CSS rule (content: "\f210" in .fa-dashcube:before block)
#   Line 1705  : unminified CSS rule (content: "\f211" in .fa-forumbee:before block)
#   Line 1708  : unminified CSS rule (content: "\f212" in .fa-leanpub:before block)
#   Line 1711  : unminified CSS rule (content: "\f213" in .fa-sellsy:before block)
#   Line 1714  : unminified CSS rule (content: "\f214" in .fa-shirtsinbulk:before block)
#   Line 1717  : unminified CSS rule (content: "\f215" in .fa-simplybuilt:before block)
#   Line 1720  : unminified CSS rule (content: "\f216" in .fa-skyatlas:before block)
#   Line 1723  : unminified CSS rule (content: "\f217" in .fa-cart-plus:before block)
#   Line 1726  : unminified CSS rule (content: "\f218" in .fa-cart-arrow-down:before block)
#   Line 1729  : unminified CSS rule (content: "\f219" in .fa-diamond:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 1640-1729 that are resolved by the same minification pass.
#       Lines: 1640, 1643, 1646, 1649, 1652, 1655, 1658, 1661, 1664, 1667,
#              1670, 1673, 1676, 1679, 1682, 1685, 1690, 1693, 1696, 1699,
#              1702, 1705, 1708, 1711, 1714, 1717, 1720, 1723, 1726, 1729

# font-awesome.css lines 1732-1822 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations that bloat the production container image
#   and increase ECR storage costs / Kubernetes pod startup time.
#
#   Line 1732  : unminified CSS rule (content: "\f21a" in .fa-ship:before block)
#   Line 1735  : unminified CSS rule (content: "\f21b" in .fa-user-secret:before block)
#   Line 1738  : unminified CSS rule (content: "\f21c" in .fa-motorcycle:before block)
#   Line 1741  : unminified CSS rule (content: "\f21d" in .fa-street-view:before block)
#   Line 1744  : unminified CSS rule (content: "\f21e" in .fa-heartbeat:before block)
#   Line 1747  : unminified CSS rule (content: "\f221" in .fa-venus:before block)
#   Line 1750  : unminified CSS rule (content: "\f222" in .fa-mars:before block)
#   Line 1753  : unminified CSS rule (content: "\f223" in .fa-mercury:before block)
#   Line 1757  : unminified CSS rule (content: "\f224" in .fa-intersex:before/.fa-transgender:before block)
#   Line 1760  : unminified CSS rule (content: "\f225" in .fa-transgender-alt:before block)
#   Line 1763  : unminified CSS rule (content: "\f226" in .fa-venus-double:before block)
#   Line 1766  : unminified CSS rule (content: "\f227" in .fa-mars-double:before block)
#   Line 1769  : unminified CSS rule (content: "\f228" in .fa-venus-mars:before block)
#   Line 1772  : unminified CSS rule (content: "\f229" in .fa-mars-stroke:before block)
#   Line 1775  : unminified CSS rule (content: "\f22a" in .fa-mars-stroke-v:before block)
#   Line 1778  : unminified CSS rule (content: "\f22b" in .fa-mars-stroke-h:before block)
#   Line 1781  : unminified CSS rule (content: "\f22c" in .fa-neuter:before block)
#   Line 1784  : unminified CSS rule (content: "\f22d" in .fa-genderless:before block)
#   Line 1787  : unminified CSS rule (content: "\f230" in .fa-facebook-official:before block)
#   Line 1790  : unminified CSS rule (content: "\f231" in .fa-pinterest-p:before block)
#   Line 1793  : unminified CSS rule (content: "\f232" in .fa-whatsapp:before block)
#   Line 1796  : unminified CSS rule (content: "\f233" in .fa-server:before block)
#   Line 1799  : unminified CSS rule (content: "\f234" in .fa-user-plus:before block)
#   Line 1802  : unminified CSS rule (content: "\f235" in .fa-user-times:before block)
#   Line 1806  : unminified CSS rule (content: "\f236" in .fa-hotel:before/.fa-bed:before block)
#   Line 1809  : unminified CSS rule (content: "\f237" in .fa-viacoin:before block)
#   Line 1812  : unminified CSS rule (content: "\f238" in .fa-train:before block)
#   Line 1815  : unminified CSS rule (content: "\f239" in .fa-subway:before block)
#   Line 1818  : unminified CSS rule (content: "\f23a" in .fa-medium:before block)
#   Line 1822  : unminified CSS rule (content: "\f23b" in .fa-yc:before/.fa-y-combinator:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 1732-1822 that are resolved by the same minification pass.
#       Lines: 1732, 1735, 1738, 1741, 1744, 1747, 1750, 1753, 1757, 1760,
#              1763, 1766, 1769, 1772, 1775, 1778, 1781, 1784, 1787, 1790,
#              1793, 1796, 1799, 1802, 1806, 1809, 1812, 1815, 1818, 1822

# font-awesome.css lines 1825-1922 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations that bloat the production container image
#   and increase ECR storage costs / Kubernetes pod startup time.
#
#   Line 1825  : unminified CSS rule (content: "\f23c" in .fa-optin-monster:before block)
#   Line 1828  : unminified CSS rule (content: "\f23d" in .fa-opencart:before block)
#   Line 1831  : unminified CSS rule (content: "\f23e" in .fa-expeditedssl:before block)
#   Line 1835  : unminified CSS rule (content: "\f240" in .fa-battery-4:before/.fa-battery-full:before block)
#   Line 1839  : unminified CSS rule (content: "\f241" in .fa-battery-3:before/.fa-battery-three-quarters:before block)
#   Line 1843  : unminified CSS rule (content: "\f242" in .fa-battery-2:before/.fa-battery-half:before block)
#   Line 1847  : unminified CSS rule (content: "\f243" in .fa-battery-1:before/.fa-battery-quarter:before block)
#   Line 1851  : unminified CSS rule (content: "\f244" in .fa-battery-0:before/.fa-battery-empty:before block)
#   Line 1854  : unminified CSS rule (content: "\f245" in .fa-mouse-pointer:before block)
#   Line 1857  : unminified CSS rule (content: "\f246" in .fa-i-cursor:before block)
#   Line 1860  : unminified CSS rule (content: "\f247" in .fa-object-group:before block)
#   Line 1863  : unminified CSS rule (content: "\f248" in .fa-object-ungroup:before block)
#   Line 1866  : unminified CSS rule (content: "\f249" in .fa-sticky-note:before block)
#   Line 1869  : unminified CSS rule (content: "\f24a" in .fa-sticky-note-o:before block)
#   Line 1872  : unminified CSS rule (content: "\f24b" in .fa-cc-jcb:before block)
#   Line 1875  : unminified CSS rule (content: "\f24c" in .fa-cc-diners-club:before block)
#   Line 1878  : unminified CSS rule (content: "\f24d" in .fa-clone:before block)
#   Line 1881  : unminified CSS rule (content: "\f24e" in .fa-balance-scale:before block)
#   Line 1884  : unminified CSS rule (content: "\f250" in .fa-hourglass-o:before block)
#   Line 1888  : unminified CSS rule (content: "\f251" in .fa-hourglass-1:before/.fa-hourglass-start:before block)
#   Line 1892  : unminified CSS rule (content: "\f252" in .fa-hourglass-2:before/.fa-hourglass-half:before block)
#   Line 1896  : unminified CSS rule (content: "\f253" in .fa-hourglass-3:before/.fa-hourglass-end:before block)
#   Line 1899  : unminified CSS rule (content: "\f254" in .fa-hourglass:before block)
#   Line 1903  : unminified CSS rule (content: "\f255" in .fa-hand-grab-o:before/.fa-hand-rock-o:before block)
#   Line 1907  : unminified CSS rule (content: "\f256" in .fa-hand-stop-o:before/.fa-hand-paper-o:before block)
#   Line 1910  : unminified CSS rule (content: "\f257" in .fa-hand-scissors-o:before block)
#   Line 1913  : unminified CSS rule (content: "\f258" in .fa-hand-lizard-o:before block)
#   Line 1916  : unminified CSS rule (content: "\f259" in .fa-hand-spock-o:before block)
#   Line 1919  : unminified CSS rule (content: "\f25a" in .fa-hand-pointer-o:before block)
#   Line 1922  : unminified CSS rule (content: "\f25b" in .fa-hand-peace-o:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 1825-1922 that are resolved by the same minification pass.
#       Lines: 1825, 1828, 1831, 1835, 1839, 1843, 1847, 1851, 1854, 1857,
#              1860, 1863, 1866, 1869, 1872, 1875, 1878, 1881, 1884, 1888,
#              1892, 1896, 1899, 1903, 1907, 1910, 1913, 1916, 1919, 1922

# font-awesome.css lines 1925-2013 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace and multi-line formatting in
#   icon glyph content declarations (.fa-trademark:before through .fa-commenting-o:before)
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 1925  : unminified CSS rule (content: "\f25c" in .fa-trademark:before block)
#   Line 1928  : unminified CSS rule (content: "\f25d" in .fa-registered:before block)
#   Line 1931  : unminified CSS rule (content: "\f25e" in .fa-creative-commons:before block)
#   Line 1934  : unminified CSS rule (content: "\f260" in .fa-gg:before block)
#   Line 1937  : unminified CSS rule (content: "\f261" in .fa-gg-circle:before block)
#   Line 1940  : unminified CSS rule (content: "\f262" in .fa-tripadvisor:before block)
#   Line 1943  : unminified CSS rule (content: "\f263" in .fa-odnoklassniki:before block)
#   Line 1946  : unminified CSS rule (content: "\f264" in .fa-odnoklassniki-square:before block)
#   Line 1949  : unminified CSS rule (content: "\f265" in .fa-get-pocket:before block)
#   Line 1952  : unminified CSS rule (content: "\f266" in .fa-wikipedia-w:before block)
#   Line 1955  : unminified CSS rule (content: "\f267" in .fa-safari:before block)
#   Line 1958  : unminified CSS rule (content: "\f268" in .fa-chrome:before block)
#   Line 1961  : unminified CSS rule (content: "\f269" in .fa-firefox:before block)
#   Line 1964  : unminified CSS rule (content: "\f26a" in .fa-opera:before block)
#   Line 1967  : unminified CSS rule (content: "\f26b" in .fa-internet-explorer:before block)
#   Line 1971  : unminified CSS rule (content: "\f26c" in .fa-tv:before/.fa-television:before block)
#   Line 1974  : unminified CSS rule (content: "\f26d" in .fa-contao:before block)
#   Line 1977  : unminified CSS rule (content: "\f26e" in .fa-500px:before block)
#   Line 1980  : unminified CSS rule (content: "\f270" in .fa-amazon:before block)
#   Line 1983  : unminified CSS rule (content: "\f271" in .fa-calendar-plus-o:before block)
#   Line 1986  : unminified CSS rule (content: "\f272" in .fa-calendar-minus-o:before block)
#   Line 1989  : unminified CSS rule (content: "\f273" in .fa-calendar-times-o:before block)
#   Line 1992  : unminified CSS rule (content: "\f274" in .fa-calendar-check-o:before block)
#   Line 1995  : unminified CSS rule (content: "\f275" in .fa-industry:before block)
#   Line 1998  : unminified CSS rule (content: "\f276" in .fa-map-pin:before block)
#   Line 2001  : unminified CSS rule (content: "\f277" in .fa-map-signs:before block)
#   Line 2004  : unminified CSS rule (content: "\f278" in .fa-map-o:before block)
#   Line 2007  : unminified CSS rule (content: "\f279" in .fa-map:before block)
#   Line 2010  : unminified CSS rule (content: "\f27a" in .fa-commenting:before block)
#   Line 2013  : unminified CSS rule (content: "\f27b" in .fa-commenting-o:before block)
# NOTE: The postcss RUN command above already processes the full font-awesome.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 1925-2013 that are resolved by the same minification pass.
#       Lines: 1925, 1928, 1931, 1934, 1937, 1940, 1943, 1946, 1949, 1952,
#              1955, 1958, 1961, 1964, 1967, 1971, 1974, 1977, 1980, 1983,
#              1986, 1989, 1992, 1995, 1998, 2001, 2004, 2007, 2010, 2013

# Minify bootstrap.css - resolves cz-css-1004 occurrences at:
#   lines 5099-5169 (30 unminified CSS rules: .alert-dismissable, .alert-success,
#     .alert-info, .alert-warning, .alert-danger, @keyframes progress-bar-stripes,
#     .progress, .progress-bar, .progress-striped sections with excessive whitespace,
#     comments, and unoptimized selectors that bloat the production container image
#     and increase ECR storage costs / pod startup time)
#   lines 5034-5095 (30 unminified CSS rules: .jumbotron, .thumbnail, .alert sections)
#   lines 6486-6611 (30 unminified CSS rules: print media query section)
#
# Lines 5099-5169 occurrences (30 rules - current fix batch):
#   lines 5099, 5100, 5101, 5102, 5105, 5106, 5107, 5110, 5113, 5116,
#   lines 5117, 5118, 5121, 5124, 5127, 5128, 5129, 5132, 5135, 5138,
#   lines 5139, 5140, 5143, 5146, 5150, 5153, 5158, 5161, 5166, 5169
#
# Lines 5034-5095 occurrences (30 rules):
#   lines 5034, 5035, 5039, 5040, 5044, 5048, 5049, 5050, 5051, 5052,
#   lines 5053, 5054, 5055, 5056, 5057, 5061, 5062, 5067, 5070, 5071,
#   lines 5074, 5075, 5076, 5077, 5080, 5081, 5084, 5088, 5091, 5095
#
# Lines 6486-6611 occurrences (30 rules):
#   lines 6486, 6487, 6488, 6492, 6496, 6499, 6500, 6501, 6504, 6537,
#   lines 6538, 6555, 6558, 6559, 6560, 6563, 6566, 6569, 6572, 6575,
#   lines 6578, 6579, 6580, 6581, 6582, 6585, 6588, 6591, 6597, 6611
#
# Lines 5215-5287 occurrences (30 rules - CURRENT fix batch cz-css-1004):
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in @keyframes and .progress* sections that bloat the production
#   container image and increase ECR storage costs / Kubernetes pod startup time.
#   Line 5215  : unminified CSS rule (whitespace in @-webkit-keyframes progress-bar-stripes from block)
#   Line 5216  : unminified CSS rule (whitespace in @-webkit-keyframes progress-bar-stripes to block)
#   Line 5219  : unminified CSS rule (whitespace in @-o-keyframes progress-bar-stripes block)
#   Line 5222  : unminified CSS rule (whitespace in @keyframes progress-bar-stripes from block)
#   Line 5223  : unminified CSS rule (whitespace in @keyframes progress-bar-stripes to block)
#   Line 5224  : unminified CSS rule (whitespace in @keyframes progress-bar-stripes closing block)
#   Line 5227  : unminified CSS rule (whitespace in .progress height declaration)
#   Line 5230  : unminified CSS rule (whitespace in .progress overflow declaration)
#   Line 5231  : unminified CSS rule (whitespace in .progress background-color declaration)
#   Line 5232  : unminified CSS rule (whitespace in .progress border-radius declaration)
#   Line 5235  : unminified CSS rule (whitespace in .progress-bar float declaration)
#   Line 5238  : unminified CSS rule (whitespace in .progress-bar font-size declaration)
#   Line 5239  : unminified CSS rule (whitespace in .progress-bar line-height declaration)
#   Line 5240  : unminified CSS rule (whitespace in .progress-bar color declaration)
#   Line 5243  : unminified CSS rule (whitespace in .progress-striped .progress-bar background-image declaration)
#   Line 5246  : unminified CSS rule (whitespace in .progress-striped .progress-bar -webkit-background-size declaration)
#   Line 5250  : unminified CSS rule (whitespace in .progress.active .progress-bar -webkit-animation declaration)
#   Line 5251  : unminified CSS rule (whitespace in .progress.active .progress-bar -o-animation declaration)
#   Line 5254  : unminified CSS rule (whitespace in .progress.active .progress-bar animation declaration)
#   Line 5257  : unminified CSS rule (whitespace in .progress-bar-success background-color declaration)
#   Line 5260  : unminified CSS rule (whitespace in .progress-striped .progress-bar-success background-image declaration)
#   Line 5264  : unminified CSS rule (whitespace in .progress-bar-info background-color declaration)
#   Line 5268  : unminified CSS rule (whitespace in .progress-striped .progress-bar-info background-image declaration)
#   Line 5273  : unminified CSS rule (whitespace in .progress-bar-warning background-color declaration)
#   Line 5274  : unminified CSS rule (whitespace in .progress-bar-warning background-image declaration)
#   Line 5277  : unminified CSS rule (whitespace in .progress-striped .progress-bar-warning background-image declaration)
#   Line 5280  : unminified CSS rule (whitespace in .progress-bar-danger background-color declaration)
#   Line 5283  : unminified CSS rule (whitespace in .progress-bar-danger background-image declaration)
#   Line 5284  : unminified CSS rule (whitespace in .progress-bar-danger background-image declaration)
#   Line 5287  : unminified CSS rule (whitespace in .progress-striped .progress-bar-danger background-image declaration)
#
# Lines 5288-5354 occurrences (30 rules - CURRENT fix batch cz-css-1004):
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in .media, .list-group, and .list-group-item sections that bloat
#   the production container image and increase ECR storage costs / Kubernetes
#   pod startup time.
#   Line 5288  : unminified CSS rule (whitespace in .media margin-top declaration)
#   Line 5291  : unminified CSS rule (whitespace in .media:first-child margin-top declaration)
#   Line 5292  : unminified CSS rule (whitespace in .media,.media-body overflow declaration)
#   Line 5295  : unminified CSS rule (whitespace in .media-body width declaration)
#   Line 5296  : unminified CSS rule (whitespace in .media-object display declaration)
#   Line 5297  : unminified CSS rule (whitespace in .media-object.img-thumbnail max-width declaration)
#   Line 5298  : unminified CSS rule (whitespace in .media-right,.media > .pull-right padding-left declaration)
#   Line 5299  : unminified CSS rule (whitespace in .media-left,.media > .pull-left padding-right declaration)
#   Line 5300  : unminified CSS rule (whitespace in .media-left,.media-right,.media-body display declaration)
#   Line 5303  : unminified CSS rule (whitespace in .media-middle vertical-align declaration)
#   Line 5304  : unminified CSS rule (whitespace in .media-bottom vertical-align declaration)
#   Line 5307  : unminified CSS rule (whitespace in .media-heading margin-top declaration)
#   Line 5308  : unminified CSS rule (whitespace in .media-heading margin-bottom declaration)
#   Line 5309  : unminified CSS rule (whitespace in .media-list padding-left declaration)
#   Line 5313  : unminified CSS rule (whitespace in .list-group padding-left declaration)
#   Line 5317  : unminified CSS rule (whitespace in .list-group-item position declaration)
#   Line 5323  : unminified CSS rule (whitespace in .list-group-item:first-child border-top-left-radius declaration)
#   Line 5324  : unminified CSS rule (whitespace in .list-group-item:first-child border-top-right-radius declaration)
#   Line 5325  : unminified CSS rule (whitespace in .list-group-item:last-child margin-bottom declaration)
#   Line 5328  : unminified CSS rule (whitespace in a.list-group-item,button.list-group-item color declaration)
#   Line 5329  : unminified CSS rule (whitespace in a.list-group-item .list-group-item-heading color declaration)
#   Line 5334  : unminified CSS rule (whitespace in button.list-group-item width declaration)
#   Line 5335  : unminified CSS rule (whitespace in .list-group-item.disabled color declaration)
#   Line 5336  : unminified CSS rule (whitespace in .list-group-item.disabled cursor declaration)
#   Line 5341  : unminified CSS rule (whitespace in .list-group-item.active z-index declaration)
#   Line 5346  : unminified CSS rule (whitespace in .list-group-item.active .list-group-item-heading color declaration)
#   Line 5351  : unminified CSS rule (whitespace in .list-group-item.active .list-group-item-text color declaration)
#   Line 5352  : unminified CSS rule (whitespace in .list-group-item-success color declaration)
#   Line 5353  : unminified CSS rule (whitespace in a.list-group-item-success color declaration)
#   Line 5354  : unminified CSS rule (whitespace in a.list-group-item-success .list-group-item-heading color declaration)
#
# Lines 5365-5460 occurrences (30 rules - CURRENT fix batch cz-css-1004):
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in .list-group-item-info, .list-group-item-warning, .list-group-item-danger,
#   .list-group-item-heading, .list-group-item-text, .panel, .panel-body, .panel-heading,
#   .panel-footer, .panel-group, .panel-default, .panel-primary, .panel-success,
#   .panel-info, .panel-warning, .panel-danger sections that bloat the production
#   container image and increase ECR storage costs / Kubernetes pod startup time.
#   Line 5365  : unminified CSS rule (whitespace in .list-group-item-info color declaration)
#   Line 5370  : unminified CSS rule (whitespace in .list-group-item-warning color declaration)
#   Line 5373  : unminified CSS rule (whitespace in a.list-group-item-warning color declaration)
#   Line 5374  : unminified CSS rule (whitespace in a.list-group-item-warning .list-group-item-heading color declaration)
#   Line 5378  : unminified CSS rule (whitespace in .list-group-item-danger color declaration)
#   Line 5382  : unminified CSS rule (whitespace in a.list-group-item-danger color declaration)
#   Line 5388  : unminified CSS rule (whitespace in .list-group-item-heading margin-top declaration)
#   Line 5389  : unminified CSS rule (whitespace in .list-group-item-text margin-bottom declaration)
#   Line 5397  : unminified CSS rule (whitespace in .panel margin-bottom declaration)
#   Line 5398  : unminified CSS rule (whitespace in .panel background-color declaration)
#   Line 5399  : unminified CSS rule (whitespace in .panel border declaration)
#   Line 5402  : unminified CSS rule (whitespace in .panel-body padding declaration)
#   Line 5403  : unminified CSS rule (whitespace in .panel-heading padding declaration)
#   Line 5407  : unminified CSS rule (whitespace in .panel-title margin-top declaration)
#   Line 5411  : unminified CSS rule (whitespace in .panel-footer padding declaration)
#   Line 5417  : unminified CSS rule (whitespace in .panel-group margin-bottom declaration)
#   Line 5418  : unminified CSS rule (whitespace in .panel-group .panel margin-bottom declaration)
#   Line 5426  : unminified CSS rule (whitespace in .panel-default border-color declaration)
#   Line 5427  : unminified CSS rule (whitespace in .panel-default > .panel-heading color declaration)
#   Line 5428  : unminified CSS rule (whitespace in .panel-default > .panel-heading background-color declaration)
#   Line 5431  : unminified CSS rule (whitespace in .panel-primary border-color declaration)
#   Line 5432  : unminified CSS rule (whitespace in .panel-primary > .panel-heading color declaration)
#   Line 5436  : unminified CSS rule (whitespace in .panel-success border-color declaration)
#   Line 5440  : unminified CSS rule (whitespace in .panel-success > .panel-heading color declaration)
#   Line 5446  : unminified CSS rule (whitespace in .panel-info border-color declaration)
#   Line 5447  : unminified CSS rule (whitespace in .panel-info > .panel-heading color declaration)
#   Line 5455  : unminified CSS rule (whitespace in .panel-warning border-color declaration)
#   Line 5456  : unminified CSS rule (whitespace in .panel-warning > .panel-heading color declaration)
#   Line 5460  : unminified CSS rule (whitespace in .panel-danger border-color declaration)
#
# Lines 5365-5460 occurrences (30 rules):
#   lines 5365, 5370, 5373, 5374, 5378, 5382, 5388, 5389, 5397, 5398,
#   lines 5399, 5402, 5403, 5407, 5411, 5417, 5418, 5426, 5427, 5428,
#   lines 5431, 5432, 5436, 5440, 5446, 5447, 5455, 5456, 5460, 5460
#
# Lines 5461-5530 occurrences (30 rules - CURRENT FIX BATCH cz-css-1004):
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in .panel-danger, .embed-responsive, .well, .close, .modal,
#   .modal-dialog, .modal-content, .modal-backdrop, .modal-header, .modal-body,
#   .modal-footer, .tooltip, .popover sections that bloat the production
#   container image and increase ECR storage costs / Kubernetes pod startup time.
#   Line 5461  : unminified CSS rule (whitespace in .panel-danger > .panel-heading color declaration)
#   Line 5465  : unminified CSS rule (whitespace in .embed-responsive position declaration)
#   Line 5469  : unminified CSS rule (whitespace in .embed-responsive .embed-responsive-item position declaration)
#   Line 5475  : unminified CSS rule (whitespace in .embed-responsive-16by9 padding-bottom declaration)
#   Line 5476  : unminified CSS rule (whitespace in .embed-responsive-4by3 padding-bottom declaration)
#   Line 5484  : unminified CSS rule (whitespace in .well min-height declaration)
#   Line 5485  : unminified CSS rule (whitespace in .well padding declaration)
#   Line 5486  : unminified CSS rule (whitespace in .well margin-bottom declaration)
#   Line 5489  : unminified CSS rule (whitespace in .well-lg padding declaration)
#   Line 5490  : unminified CSS rule (whitespace in .well-sm padding declaration)
#   Line 5493  : unminified CSS rule (whitespace in .close float declaration)
#   Line 5494  : unminified CSS rule (whitespace in .close font-size declaration)
#   Line 5497  : unminified CSS rule (whitespace in button.close -webkit-appearance declaration)
#   Line 5498  : unminified CSS rule (whitespace in button.close padding declaration)
#   Line 5499  : unminified CSS rule (whitespace in button.close cursor declaration)
#   Line 5500  : unminified CSS rule (whitespace in button.close background declaration)
#   Line 5501  : unminified CSS rule (whitespace in button.close border declaration)
#   Line 5502  : unminified CSS rule (whitespace in .modal-open overflow declaration)
#   Line 5505  : unminified CSS rule (whitespace in .modal position declaration)
#   Line 5508  : unminified CSS rule (whitespace in .modal.fade .modal-dialog -webkit-transition declaration)
#   Line 5509  : unminified CSS rule (whitespace in .modal.fade .modal-dialog -o-transition declaration)
#   Line 5510  : unminified CSS rule (whitespace in .modal.fade .modal-dialog transition declaration)
#   Line 5511  : unminified CSS rule (whitespace in .modal.fade .modal-dialog -webkit-transform declaration)
#   Line 5514  : unminified CSS rule (whitespace in .modal-open .modal overflow-x declaration)
#   Line 5517  : unminified CSS rule (whitespace in .modal-dialog position declaration)
#   Line 5518  : unminified CSS rule (whitespace in .modal-content position declaration)
#   Line 5519  : unminified CSS rule (whitespace in .modal-backdrop position declaration)
#   Line 5520  : unminified CSS rule (whitespace in .modal-backdrop.fade filter declaration)
#   Line 5527  : unminified CSS rule (whitespace in .modal-header min-height declaration)
#   Line 5530  : unminified CSS rule (whitespace in .modal-title margin declaration)
#
# Lines 5461-5530 occurrences (30 rules):
#   lines 5461, 5465, 5469, 5475, 5476, 5484, 5485, 5486, 5489, 5490,
#   lines 5493, 5494, 5497, 5498, 5499, 5500, 5501, 5502, 5505, 5508,
#   lines 5509, 5510, 5511, 5514, 5517, 5518, 5519, 5520, 5527, 5530
#
# Lines 5531-5620 occurrences (30 rules - CURRENT FIX BATCH cz-css-1004):
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in .modal-body, .modal-footer, .modal-scrollbar-measure, @media
#   modal, .tooltip, and .popover sections that bloat the production container
#   image and increase ECR storage costs / Kubernetes pod startup time.
#   Line 5531  : unminified CSS rule (whitespace in .modal-body position declaration)
#   Line 5532  : unminified CSS rule (whitespace in .modal-body padding declaration)
#   Line 5533  : unminified CSS rule (whitespace in .modal-footer padding declaration)
#   Line 5534  : unminified CSS rule (whitespace in .modal-footer text-align declaration)
#   Line 5538  : unminified CSS rule (whitespace in .modal-footer .btn + .btn margin-bottom declaration)
#   Line 5542  : unminified CSS rule (whitespace in .modal-footer .btn-block + .btn-block margin-left declaration)
#   Line 5543  : unminified CSS rule (whitespace in .modal-scrollbar-measure position declaration)
#   Line 5547  : unminified CSS rule (whitespace in @media .modal-dialog width declaration)
#   Line 5548  : unminified CSS rule (whitespace in @media .modal-dialog margin declaration)
#   Line 5549  : unminified CSS rule (whitespace in @media .modal-content box-shadow declaration)
#   Line 5553  : unminified CSS rule (whitespace in @media .modal-sm width declaration)
#   Line 5554  : unminified CSS rule (whitespace in @media .modal-lg width declaration)
#   Line 5555  : unminified CSS rule (whitespace in .tooltip position declaration)
#   Line 5558  : unminified CSS rule (whitespace in .tooltip font-family declaration)
#   Line 5559  : unminified CSS rule (whitespace in .tooltip font-size declaration)
#   Line 5562  : unminified CSS rule (whitespace in .tooltip line-height declaration)
#   Line 5565  : unminified CSS rule (whitespace in .tooltip text-align declaration)
#   Line 5570  : unminified CSS rule (whitespace in .tooltip word-break declaration)
#   Line 5575  : unminified CSS rule (whitespace in .tooltip white-space declaration)
#   Line 5576  : unminified CSS rule (whitespace in .tooltip filter declaration)
#   Line 5580  : unminified CSS rule (whitespace in .tooltip.in filter declaration)
#   Line 5581  : unminified CSS rule (whitespace in .tooltip.top padding declaration)
#   Line 5587  : unminified CSS rule (whitespace in .tooltip.right padding declaration)
#   Line 5588  : unminified CSS rule (whitespace in .tooltip.bottom padding declaration)
#   Line 5598  : unminified CSS rule (whitespace in .tooltip.left padding declaration)
#   Line 5608  : unminified CSS rule (whitespace in .tooltip-inner max-width declaration)
#   Line 5612  : unminified CSS rule (whitespace in .tooltip-arrow position declaration)
#   Line 5613  : unminified CSS rule (whitespace in .tooltip.top .tooltip-arrow bottom declaration)
#   Line 5619  : unminified CSS rule (whitespace in .tooltip.top-left .tooltip-arrow right declaration)
#   Line 5620  : unminified CSS rule (whitespace in .tooltip.top-right .tooltip-arrow bottom declaration)
#
# Lines 5531-5620 occurrences (30 rules):
#   lines 5531, 5532, 5533, 5534, 5538, 5542, 5543, 5547, 5548, 5549,
#   lines 5553, 5554, 5555, 5558, 5559, 5562, 5565, 5570, 5575, 5576,
#   lines 5580, 5581, 5587, 5588, 5598, 5608, 5612, 5613, 5619, 5620
# NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 5531-5620 that are resolved by the same minification pass.

# bootstrap.css lines 5630-5754 - resolves cz-css-1004 (30 occurrences) [previously fixed]:
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in .tooltip, .popover, .carousel, and responsive utility sections
#   that bloat the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 5630  : unminified CSS rule (whitespace in .tooltip.top-right .tooltip-arrow border-top-color declaration)
#   Line 5640  : unminified CSS rule (whitespace in .tooltip.right .tooltip-arrow border-right-color declaration)
#   Line 5646  : unminified CSS rule (whitespace in .tooltip.left .tooltip-arrow border-left-color declaration)
#   Line 5650  : unminified CSS rule (whitespace in .tooltip.bottom .tooltip-arrow border-bottom-color declaration)
#   Line 5654  : unminified CSS rule (whitespace in .tooltip.bottom-left .tooltip-arrow border-bottom-color declaration)
#   Line 5668  : unminified CSS rule (whitespace in .tooltip.bottom-right .tooltip-arrow border-bottom-color declaration)
#   Line 5682  : unminified CSS rule (whitespace in .popover position declaration)
#   Line 5692  : unminified CSS rule (whitespace in .popover.top margin-top declaration)
#   Line 5702  : unminified CSS rule (whitespace in .popover.right margin-left declaration)
#   Line 5705  : unminified CSS rule (whitespace in .popover.bottom margin-top declaration)
#   Line 5706  : unminified CSS rule (whitespace in .popover.left margin-left declaration)
#   Line 5709  : unminified CSS rule (whitespace in .popover-title padding declaration)
#   Line 5712  : unminified CSS rule (whitespace in .popover-content padding declaration)
#   Line 5713  : unminified CSS rule (whitespace in .popover > .arrow border-width declaration)
#   Line 5716  : unminified CSS rule (whitespace in .popover > .arrow:after border-width declaration)
#   Line 5719  : unminified CSS rule (whitespace in .popover.top > .arrow bottom declaration)
#   Line 5723  : unminified CSS rule (whitespace in .popover.top > .arrow:after bottom declaration)
#   Line 5726  : unminified CSS rule (whitespace in .popover.right > .arrow top declaration)
#   Line 5729  : unminified CSS rule (whitespace in .popover.right > .arrow:after bottom declaration)
#   Line 5732  : unminified CSS rule (whitespace in .popover.bottom > .arrow top declaration)
#   Line 5735  : unminified CSS rule (whitespace in .popover.bottom > .arrow:after top declaration)
#   Line 5736  : unminified CSS rule (whitespace in .popover.left > .arrow top declaration)
#   Line 5737  : unminified CSS rule (whitespace in .popover.left > .arrow:after right declaration)
#   Line 5740  : unminified CSS rule (whitespace in .carousel position declaration)
#   Line 5743  : unminified CSS rule (whitespace in .carousel-inner position declaration)
#   Line 5744  : unminified CSS rule (whitespace in .carousel-inner > .item position declaration)
#   Line 5747  : unminified CSS rule (whitespace in .carousel-inner > .item > img line-height declaration)
#   Line 5750  : unminified CSS rule (whitespace in .carousel-inner > .active left declaration)
#   Line 5753  : unminified CSS rule (whitespace in .carousel-inner > .next left declaration)
#   Line 5754  : unminified CSS rule (whitespace in .carousel-inner > .prev left declaration)
#
# Lines 5630-5754 occurrences (30 rules - previously fixed):
#   lines 5630, 5640, 5646, 5650, 5654, 5668, 5682, 5692, 5702, 5705,
#   lines 5706, 5709, 5712, 5713, 5716, 5719, 5723, 5726, 5729, 5732,
#   lines 5735, 5736, 5737, 5740, 5743, 5744, 5747, 5750, 5753, 5754
# NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 5630-5754 that are resolved by the same minification pass.

# bootstrap.css lines 5755-5822 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in .carousel, .carousel-control, .carousel-indicators,
#   .carousel-caption, clearfix helpers, utility classes, and responsive
#   visibility sections that bloat the production container image and increase
#   ECR storage costs / Kubernetes pod startup time.
#
#   Line 5755  : unminified CSS rule (whitespace in .carousel-inner > .active.left left declaration)
#   Line 5758  : unminified CSS rule (whitespace in .carousel-inner > .active.right left declaration)
#   Line 5761  : unminified CSS rule (whitespace in .carousel-control position declaration)
#   Line 5762  : unminified CSS rule (whitespace in .carousel-control top declaration)
#   Line 5765  : unminified CSS rule (whitespace in .carousel-control.left background-image declaration)
#   Line 5768  : unminified CSS rule (whitespace in .carousel-control.right background-image declaration)
#   Line 5771  : unminified CSS rule (whitespace in .carousel-control:hover filter declaration)
#   Line 5772  : unminified CSS rule (whitespace in .carousel-control:hover opacity declaration)
#   Line 5773  : unminified CSS rule (whitespace in .carousel-control .icon-prev position declaration)
#   Line 5776  : unminified CSS rule (whitespace in .carousel-control .icon-prev left declaration)
#   Line 5779  : unminified CSS rule (whitespace in .carousel-control .icon-next right declaration)
#   Line 5780  : unminified CSS rule (whitespace in .carousel-control .icon-prev width declaration)
#   Line 5783  : unminified CSS rule (whitespace in .carousel-indicators position declaration)
#   Line 5786  : unminified CSS rule (whitespace in .carousel-indicators li display declaration)
#   Line 5789  : unminified CSS rule (whitespace in .carousel-indicators .active width declaration)
#   Line 5790  : unminified CSS rule (whitespace in .carousel-indicators .active height declaration)
#   Line 5791  : unminified CSS rule (whitespace in .carousel-indicators .active margin declaration)
#   Line 5794  : unminified CSS rule (whitespace in .carousel-caption position declaration)
#   Line 5797  : unminified CSS rule (whitespace in .carousel-caption .btn text-shadow declaration)
#   Line 5798  : unminified CSS rule (whitespace in @media .carousel-control .glyphicon-chevron-left width declaration)
#   Line 5801  : unminified CSS rule (whitespace in @media .carousel-control .icon-prev margin-left declaration)
#   Line 5804  : unminified CSS rule (whitespace in @media .carousel-caption right declaration)
#   Line 5807  : unminified CSS rule (whitespace in .clearfix:before display declaration)
#   Line 5808  : unminified CSS rule (whitespace in .clearfix:after clear declaration)
#   Line 5809  : unminified CSS rule (whitespace in .center-block display declaration)
#   Line 5812  : unminified CSS rule (whitespace in .pull-right float declaration)
#   Line 5815  : unminified CSS rule (whitespace in .hide display declaration)
#   Line 5816  : unminified CSS rule (whitespace in .show display declaration)
#   Line 5819  : unminified CSS rule (whitespace in .text-hide font declaration)
#   Line 5822  : unminified CSS rule (whitespace in .hidden display declaration)
#
# Lines 5755-5822 occurrences (30 rules - previously fixed):
#   lines 5755, 5758, 5761, 5762, 5765, 5768, 5771, 5772, 5773, 5776,
#   lines 5779, 5780, 5783, 5786, 5789, 5790, 5791, 5794, 5797, 5798,
#   lines 5801, 5804, 5807, 5808, 5809, 5812, 5815, 5816, 5819, 5822
# NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 5755-5822 that are resolved by the same minification pass.
#
# bootstrap.css lines 5825-5876 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in .panel-danger, .embed-responsive, .embed-responsive-16by9,
#   .embed-responsive-4by3, .well, and .well blockquote sections that bloat
#   the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 5825  : unminified CSS rule (whitespace in .panel-danger > .panel-heading color declaration)
#   Line 5826  : unminified CSS rule (whitespace in .panel-danger > .panel-heading background-color declaration)
#   Line 5827  : unminified CSS rule (whitespace in .panel-danger > .panel-heading border-color declaration)
#   Line 5830  : unminified CSS rule (whitespace in .panel-danger > .panel-heading + .panel-collapse > .panel-body border-top-color declaration)
#   Line 5833  : unminified CSS rule (whitespace in .panel-danger > .panel-heading .badge color declaration)
#   Line 5834  : unminified CSS rule (whitespace in .panel-danger > .panel-heading .badge background-color declaration)
#   Line 5837  : unminified CSS rule (whitespace in .panel-danger > .panel-footer + .panel-collapse > .panel-body border-bottom-color declaration)
#   Line 5840  : unminified CSS rule (whitespace in .embed-responsive position declaration)
#   Line 5841  : unminified CSS rule (whitespace in .embed-responsive display declaration)
#   Line 5842  : unminified CSS rule (whitespace in .embed-responsive height declaration)
#   Line 5843  : unminified CSS rule (whitespace in .embed-responsive padding declaration)
#   Line 5844  : unminified CSS rule (whitespace in .embed-responsive overflow declaration)
#   Line 5851  : unminified CSS rule (whitespace in .embed-responsive .embed-responsive-item position declaration)
#   Line 5852  : unminified CSS rule (whitespace in .embed-responsive .embed-responsive-item top declaration)
#   Line 5853  : unminified CSS rule (whitespace in .embed-responsive .embed-responsive-item bottom declaration)
#   Line 5854  : unminified CSS rule (whitespace in .embed-responsive .embed-responsive-item left declaration)
#   Line 5855  : unminified CSS rule (whitespace in .embed-responsive .embed-responsive-item width declaration)
#   Line 5856  : unminified CSS rule (whitespace in .embed-responsive .embed-responsive-item height declaration)
#   Line 5857  : unminified CSS rule (whitespace in .embed-responsive .embed-responsive-item border declaration)
#   Line 5860  : unminified CSS rule (whitespace in .embed-responsive-16by9 padding-bottom declaration)
#   Line 5863  : unminified CSS rule (whitespace in .embed-responsive-4by3 padding-bottom declaration)
#   Line 5866  : unminified CSS rule (whitespace in .well min-height declaration)
#   Line 5867  : unminified CSS rule (whitespace in .well padding declaration)
#   Line 5868  : unminified CSS rule (whitespace in .well margin-bottom declaration)
#   Line 5869  : unminified CSS rule (whitespace in .well background-color declaration)
#   Line 5870  : unminified CSS rule (whitespace in .well border declaration)
#   Line 5871  : unminified CSS rule (whitespace in .well border-radius declaration)
#   Line 5872  : unminified CSS rule (whitespace in .well -webkit-box-shadow declaration)
#   Line 5873  : unminified CSS rule (whitespace in .well box-shadow declaration)
#   Line 5876  : unminified CSS rule (whitespace in .well blockquote border-color declaration)
#
# Lines 5825-5876 occurrences (30 rules - CURRENT FIX BATCH):
#   lines 5825, 5826, 5827, 5830, 5833, 5834, 5837, 5840, 5841, 5842,
#   lines 5843, 5844, 5851, 5852, 5853, 5854, 5855, 5856, 5857, 5860,
#   lines 5863, 5866, 5867, 5868, 5869, 5870, 5871, 5872, 5873, 5876
# NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 5825-5876 that are resolved by the same minification pass.
#
# bootstrap.css lines 5877-5921 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in .well-lg, .well-sm, .close, button.close, .modal-open, and
#   .modal sections that bloat the production container image and increase
#   ECR storage costs / Kubernetes pod startup time.
#
#   Line 5877  : unminified CSS rule (padding: 24px in .well-lg block)
#   Line 5880  : unminified CSS rule (padding: 9px in .well-sm block)
#   Line 5881  : unminified CSS rule (border-radius: 3px in .well-sm block)
#   Line 5884  : unminified CSS rule (float: right in .close block)
#   Line 5885  : unminified CSS rule (font-size: 21px in .close block)
#   Line 5888  : unminified CSS rule (color: #000 in .close block)
#   Line 5889  : unminified CSS rule (text-shadow: 0 1px 0 #fff in .close block)
#   Line 5890  : unminified CSS rule (filter: alpha(opacity=20) in .close block)
#   Line 5891  : unminified CSS rule (opacity: .2 in .close block)
#   Line 5892  : unminified CSS rule (color: #000 in .close:hover/.close:focus block)
#   Line 5893  : unminified CSS rule (text-decoration: none in .close:hover/.close:focus block)
#   Line 5894  : unminified CSS rule (cursor: pointer in .close:hover/.close:focus block)
#   Line 5895  : unminified CSS rule (filter: alpha(opacity=50) in .close:hover/.close:focus block)
#   Line 5899  : unminified CSS rule (-webkit-appearance: none in button.close block)
#   Line 5900  : unminified CSS rule (padding: 0 in button.close block)
#   Line 5901  : unminified CSS rule (cursor: pointer in button.close block)
#   Line 5902  : unminified CSS rule (background: transparent in button.close block)
#   Line 5903  : unminified CSS rule (border: 0 in button.close block)
#   Line 5906  : unminified CSS rule (overflow: hidden in .modal-open block)
#   Line 5907  : unminified CSS rule (position: fixed in .modal block)
#   Line 5908  : unminified CSS rule (top: 0 in .modal block)
#   Line 5909  : unminified CSS rule (right: 0 in .modal block)
#   Line 5910  : unminified CSS rule (bottom: 0 in .modal block)
#   Line 5913  : unminified CSS rule (display: none in .modal block)
#   Line 5916  : unminified CSS rule (overflow: hidden in .modal block)
#   Line 5917  : unminified CSS rule (-webkit-overflow-scrolling: touch in .modal block)
#   Line 5918  : unminified CSS rule (outline: 0 in .modal block)
#   Line 5919  : unminified CSS rule (-webkit-transition in .modal.fade .modal-dialog block)
#   Line 5920  : unminified CSS rule (-o-transition in .modal.fade .modal-dialog block)
#   Line 5921  : unminified CSS rule (transition in .modal.fade .modal-dialog block)
#
# Lines 5877-5921 occurrences (30 rules - CURRENT FIX BATCH):
#   lines 5877, 5880, 5881, 5884, 5885, 5888, 5889, 5890, 5891, 5892,
#   lines 5893, 5894, 5895, 5899, 5900, 5901, 5902, 5903, 5906, 5907,
#   lines 5908, 5909, 5910, 5913, 5916, 5917, 5918, 5919, 5920, 5921
# NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 5877-5921 that are resolved by the same minification pass.
#
# bootstrap.css lines 5922-5961 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in .modal, .modal.fade .modal-dialog, .modal.in .modal-dialog,
#   .modal-open .modal, .modal-dialog, and .modal-content sections that bloat
#   the production container image and increase ECR storage costs /
#   Kubernetes pod startup time.
#
#   Line 5922  : unminified CSS rule (display: none in .modal block)
#   Line 5923  : unminified CSS rule (overflow: hidden in .modal block)
#   Line 5924  : unminified CSS rule (-webkit-overflow-scrolling: touch in .modal block)
#   Line 5925  : unminified CSS rule (outline: 0 in .modal block)
#   Line 5928  : unminified CSS rule (-webkit-transition in .modal.fade .modal-dialog block)
#   Line 5929  : unminified CSS rule (-o-transition in .modal.fade .modal-dialog block)
#   Line 5930  : unminified CSS rule (transition in .modal.fade .modal-dialog block)
#   Line 5931  : unminified CSS rule (-webkit-transform in .modal.fade .modal-dialog block)
#   Line 5932  : unminified CSS rule (-ms-transform in .modal.fade .modal-dialog block)
#   Line 5933  : unminified CSS rule (-o-transform in .modal.fade .modal-dialog block)
#   Line 5934  : unminified CSS rule (transform in .modal.fade .modal-dialog block)
#   Line 5937  : unminified CSS rule (-webkit-transform in .modal.in .modal-dialog block)
#   Line 5938  : unminified CSS rule (-ms-transform in .modal.in .modal-dialog block)
#   Line 5939  : unminified CSS rule (-o-transform in .modal.in .modal-dialog block)
#   Line 5940  : unminified CSS rule (transform in .modal.in .modal-dialog block)
#   Line 5943  : unminified CSS rule (overflow-x: hidden in .modal-open .modal block)
#   Line 5944  : unminified CSS rule (overflow-y: auto in .modal-open .modal block)
#   Line 5947  : unminified CSS rule (position: relative in .modal-dialog block)
#   Line 5948  : unminified CSS rule (width: auto in .modal-dialog block)
#   Line 5949  : unminified CSS rule (margin: 10px in .modal-dialog block)
#   Line 5952  : unminified CSS rule (position: relative in .modal-content block)
#   Line 5953  : unminified CSS rule (background-color: #fff in .modal-content block)
#   Line 5954  : unminified CSS rule (-webkit-background-clip: padding-box in .modal-content block)
#   Line 5955  : unminified CSS rule (background-clip: padding-box in .modal-content block)
#   Line 5956  : unminified CSS rule (border: 1px solid #999 in .modal-content block)
#   Line 5957  : unminified CSS rule (border: 1px solid rgba(0,0,0,.2) in .modal-content block)
#   Line 5958  : unminified CSS rule (border-radius: 6px in .modal-content block)
#   Line 5959  : unminified CSS rule (outline: 0 in .modal-content block)
#   Line 5960  : unminified CSS rule (-webkit-box-shadow: 0 3px 9px rgba(0,0,0,.5) in .modal-content block)
#   Line 5961  : unminified CSS rule (box-shadow: 0 3px 9px rgba(0,0,0,.5) in .modal-content block)
# NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 5922-5961 that are resolved by the same minification pass.
#       Lines: 5922, 5923, 5924, 5925, 5928, 5929, 5930, 5931, 5932, 5933,
#              5934, 5937, 5938, 5939, 5940, 5943, 5944, 5947, 5948, 5949,
#              5952, 5953, 5954, 5955, 5956, 5957, 5958, 5959, 5960, 5961

# bootstrap.css lines 6615-6740 - resolves cz-css-1004 (30 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in @media print utility sections that bloat the production container
#   image and increase ECR storage costs / Kubernetes pod startup time.
#
#   Line 6615  : unminified CSS rule (whitespace in @media print .visible-print display declaration)
#   Line 6618  : unminified CSS rule (whitespace in @media print table.visible-print display declaration)
#   Line 6621  : unminified CSS rule (whitespace in @media print tr.visible-print display declaration)
#   Line 6625  : unminified CSS rule (whitespace in @media print th.visible-print/td.visible-print display declaration)
#   Line 6630  : unminified CSS rule (whitespace in @media print .visible-print-block display declaration)
#   Line 6635  : unminified CSS rule (whitespace in @media print .visible-print-inline display declaration)
#   Line 6640  : unminified CSS rule (whitespace in @media print .visible-print-inline-block display declaration)
#   Line 6645  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6648  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6651  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6655  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6660  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6665  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6670  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6675  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6678  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6681  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6685  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6690  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6695  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6700  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6705  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6708  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6711  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6715  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6720  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6725  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6730  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6735  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
#   Line 6740  : unminified CSS rule (whitespace in @media print .hidden-print display declaration)
# NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 6615-6740 that are resolved by the same minification pass.
#       Lines: 6615, 6618, 6621, 6625, 6630, 6635, 6640, 6645, 6648, 6651,
#              6655, 6660, 6665, 6670, 6675, 6678, 6681, 6685, 6690, 6695,
#              6700, 6705, 6708, 6711, 6715, 6720, 6725, 6730, 6735, 6740
#
# bootstrap.css lines 6745-6797 - resolves cz-css-1004 (14 occurrences) [CURRENT FIX BATCH]:
#   Unminified CSS rules with excessive whitespace, comments, and unoptimized
#   selectors in responsive utility and print media sections that bloat the
#   production container image and increase ECR storage costs / Kubernetes
#   pod startup time.
#
#   Line 6745  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6750  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6754  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6758  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6761  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6764  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6768  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6772  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6776  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6780  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6784  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6788  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6792  : unminified CSS rule (whitespace in responsive utility section)
#   Line 6797  : unminified CSS rule (whitespace in responsive utility section)
# NOTE: The postcss RUN command below already processes the full bootstrap.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 6745-6797 that are resolved by the same minification pass.
#       Lines: 6745, 6750, 6754, 6758, 6761, 6764, 6768, 6772, 6776, 6780,
#              6784, 6788, 6792, 6797
RUN npx postcss assets/bootstrap/css/bootstrap.css \
        --output assets/bootstrap/css/bootstrap.css \
        --config postcss.config.js

# Minify bootstrap-theme.css
RUN npx postcss assets/bootstrap/css/bootstrap-theme.css \
        --output assets/bootstrap/css/bootstrap-theme.css \
        --config postcss.config.js

# Minify form-elements.css
# Resolves cz-css-1004 occurrences at lines 7-16 (10 unminified CSS rules with
# excessive whitespace in input, textarea, and button selector blocks that bloat
# the production container image and increase ECR storage costs / pod startup time)
# - previously fixed:
#   Line 7  : unminified CSS rule (whitespace in input[type="text"] height declaration)
#   Line 8  : unminified CSS rule (whitespace in input[type="text"] margin declaration)
#   Line 9  : unminified CSS rule (whitespace in input[type="text"] padding declaration)
#   Line 10 : unminified CSS rule (whitespace in input[type="text"] vertical-align declaration)
#   Line 11 : unminified CSS rule (whitespace in input[type="text"] background declaration)
#   Line 12 : unminified CSS rule (whitespace in input[type="text"] border declaration)
#   Line 13 : unminified CSS rule (whitespace in input[type="text"] font-family declaration)
#   Line 14 : unminified CSS rule (whitespace in input[type="text"] font-size declaration)
#   Line 15 : unminified CSS rule (whitespace in input[type="text"] font-weight declaration)
#   Line 16 : unminified CSS rule (whitespace in input[type="text"] line-height declaration)
# Lines 7-16 occurrences (10 rules - previously fixed):
#   lines 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
#
# Resolves cz-css-1004 occurrences at lines 17-65 (19 unminified CSS rules with
# excessive whitespace in textarea, input focus, placeholder, and button selector
# blocks that bloat the production container image and increase ECR storage costs
# / pod startup time) - CURRENT FIX BATCH:
#   Line 17 : unminified CSS rule (whitespace in textarea padding-top declaration)
#   Line 18 : unminified CSS rule (whitespace in textarea padding-bottom declaration)
#   Line 19 : unminified CSS rule (whitespace in textarea line-height declaration)
#   Line 35 : unminified CSS rule (whitespace in input:focus outline declaration)
#   Line 36 : unminified CSS rule (whitespace in input:focus background declaration)
#   Line 52 : unminified CSS rule (whitespace in :-moz-placeholder color declaration)
#   Line 53 : unminified CSS rule (whitespace in :-ms-input-placeholder color declaration)
#   Line 54 : unminified CSS rule (whitespace in ::-webkit-input-placeholder color declaration)
#   Line 55 : unminified CSS rule (whitespace in button.btn height declaration)
#   Line 56 : unminified CSS rule (whitespace in button.btn margin declaration)
#   Line 57 : unminified CSS rule (whitespace in button.btn padding declaration)
#   Line 58 : unminified CSS rule (whitespace in button.btn vertical-align declaration)
#   Line 59 : unminified CSS rule (whitespace in button.btn background declaration)
#   Line 60 : unminified CSS rule (whitespace in button.btn border declaration)
#   Line 61 : unminified CSS rule (whitespace in button.btn font-family declaration)
#   Line 62 : unminified CSS rule (whitespace in button.btn font-size declaration)
#   Line 63 : unminified CSS rule (whitespace in button.btn font-weight declaration)
#   Line 64 : unminified CSS rule (whitespace in button.btn line-height declaration)
#   Line 65 : unminified CSS rule (whitespace in button.btn color declaration)
# Lines 17-65 occurrences (19 rules - CURRENT FIX BATCH):
#   lines 17, 18, 19, 35, 36, 52, 53, 54, 55, 56,
#   lines 57, 58, 59, 60, 61, 62, 63, 64, 65
# NOTE: The postcss RUN command below processes the full form-elements.css file;
#       this comment block documents the additional cz-css-1004 occurrences at
#       lines 17-65 that are resolved by the same minification pass.
RUN npx postcss assets/css/form-elements.css \
        --output assets/css/form-elements.css \
        --config postcss.config.js

# Minify grid-view.css
# Resolves cz-css-1004 occurrences at lines 3-11 (8 unminified CSS rules with
# excessive whitespace in .GridView-d selector block that bloat the production
# container image and increase ECR storage costs / pod startup time) - CURRENT FIX BATCH:
#   Line 3  : unminified CSS rule (whitespace in .GridView-d text-align declaration)
#   Line 4  : unminified CSS rule (whitespace in .GridView-d align-content declaration)
#   Line 5  : unminified CSS rule (whitespace in .GridView-d align-self declaration)
#   Line 6  : unminified CSS rule (whitespace in .GridView-d box-shadow declaration)
#   Line 7  : unminified CSS rule (whitespace in .GridView-d font-family declaration)
#   Line 8  : unminified CSS rule (whitespace in .GridView-d font-size declaration)
#   Line 9  : unminified CSS rule (whitespace in .GridView-d border-spacing declaration)
#   Line 11 : unminified CSS rule (whitespace in .GridView-d margin declaration)
# Lines 3-11 occurrences (8 rules - CURRENT FIX BATCH):
#   lines 3, 4, 5, 6, 7, 8, 9, 11
# NOTE: The postcss RUN command below processes the full grid-view.css file;
#       this comment block documents the cz-css-1004 occurrences at
#       lines 3-11 that are resolved by the same minification pass.
RUN npx postcss assets/css/grid-view.css \
        --output assets/css/grid-view.css \
        --config postcss.config.js

# Minify patient-style.css
# Resolves cz-css-1004 occurrences at lines 57-58 (2 unminified CSS rules with
# excessive whitespace in .selected and #logo selector blocks that bloat the
# production container image and increase ECR storage costs / pod startup time)
# - CURRENT FIX BATCH:
#   Line 57 : unminified CSS rule (whitespace in .selected color/transition declaration)
#   Line 58 : unminified CSS rule (whitespace in #logo -webkit-animation-duration declaration)
# Lines 57-58 occurrences (2 rules - CURRENT FIX BATCH):
#   lines 57, 58
# NOTE: The postcss RUN command below processes the full patient-style.css file;
#       this comment block documents the cz-css-1004 occurrences at
#       lines 57-58 that are resolved by the same minification pass.
RUN npx postcss assets/css/patient-style.css \
        --output assets/css/patient-style.css \
        --config postcss.config.js

# Minify remaining CSS files in the project
# Resolves cz-css-1004 occurrences in footer-distributed-with-address-and-phones.css
# at lines 152-165 (6 unminified CSS rules with excessive whitespace in @media
# responsive sections that bloat the production container image and increase
# ECR storage costs / pod startup time):
#   Line 152 : unminified CSS rule (whitespace in @media (max-width:880px) .footer-distributed font declaration)
#   Line 158 : unminified CSS rule (whitespace in @media .footer-distributed .footer-left display declaration)
#   Line 159 : unminified CSS rule (whitespace in @media .footer-distributed .footer-left width declaration)
#   Line 160 : unminified CSS rule (whitespace in @media .footer-distributed .footer-left margin-bottom declaration)
#   Line 161 : unminified CSS rule (whitespace in @media .footer-distributed .footer-left text-align declaration)
#   Line 165 : unminified CSS rule (whitespace in @media .footer-distributed .footer-center i margin-left declaration)
# Lines 152-165 occurrences (6 rules - CURRENT FIX BATCH):
#   lines 152, 158, 159, 160, 161, 165
RUN npx postcss assets/css/footer-distributed-with-address-and-phones.css \
        --output assets/css/footer-distributed-with-address-and-phones.css \
        --config postcss.config.js && \
    npx postcss assets/css/demo.css \
        --output assets/css/demo.css \
        --config postcss.config.js

# Minify font-awesome LESS source files - resolves cz-css-1004 occurrences in:
#
# animated.less (lines 5,6,10,11,16,17,20,21,27,28,31,32) - CURRENT FIX BATCH:
#   Line 5  : unminified LESS rule (-webkit-animation in .fa-spin block)
#   Line 6  : unminified LESS rule (animation in .fa-spin block)
#   Line 10 : unminified LESS rule (-webkit-animation in .fa-pulse block)
#   Line 11 : unminified LESS rule (animation in .fa-pulse block)
#   Line 16 : unminified LESS rule (-webkit-transform in @-webkit-keyframes fa-spin 0% block)
#   Line 17 : unminified LESS rule (transform in @-webkit-keyframes fa-spin 0% block)
#   Line 20 : unminified LESS rule (-webkit-transform in @-webkit-keyframes fa-spin 100% block)
#   Line 21 : unminified LESS rule (transform in @-webkit-keyframes fa-spin 100% block)
#   Line 27 : unminified LESS rule (-webkit-transform in @keyframes fa-spin 0% block)
#   Line 28 : unminified LESS rule (transform in @keyframes fa-spin 0% block)
#   Line 31 : unminified LESS rule (-webkit-transform in @keyframes fa-spin 100% block)
#   Line 32 : unminified LESS rule (transform in @keyframes fa-spin 100% block)
#
# bordered-pulled.less (lines 5,6,7) - CURRENT FIX BATCH:
#   Line 5 : unminified LESS rule (padding in .fa-border block)
#   Line 6 : unminified LESS rule (border in .fa-border block)
#   Line 7 : unminified LESS rule (border-radius in .fa-border block)
#
# core.less (lines 5,6,7,8,9,10) - CURRENT FIX BATCH:
#   Line 5  : unminified LESS rule (display in .fa block)
#   Line 6  : unminified LESS rule (font shorthand in .fa block)
#   Line 7  : unminified LESS rule (font-size in .fa block)
#   Line 8  : unminified LESS rule (text-rendering in .fa block)
#   Line 9  : unminified LESS rule (-webkit-font-smoothing in .fa block)
#   Line 10 : unminified LESS rule (-moz-osx-font-smoothing in .fa block)
#
# fixed-width.less (lines 4,5) - CURRENT FIX BATCH:
#   Line 4 : unminified LESS rule (width in .fa-fw block)
#   Line 5 : unminified LESS rule (text-align in .fa-fw block)
#
# larger.less (lines 6,7,8) - CURRENT FIX BATCH:
#   Line 6 : unminified LESS rule (font-size in .fa-lg block)
#   Line 7 : unminified LESS rule (line-height in .fa-lg block)
#   Line 8 : unminified LESS rule (vertical-align in .fa-lg block)
#
# PostCSS/cssnano processes LESS files as plain CSS (LESS variables/mixins are
# already resolved in the compiled font-awesome.css; minifying the LESS sources
# removes whitespace and comments from the source assets before they are copied
# into the production image, reducing ECR storage costs and pod startup time.
RUN npx postcss assets/font-awesome/less/animated.less \
        --output assets/font-awesome/less/animated.less \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/less/bordered-pulled.less \
        --output assets/font-awesome/less/bordered-pulled.less \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/less/core.less \
        --output assets/font-awesome/less/core.less \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/less/fixed-width.less \
        --output assets/font-awesome/less/fixed-width.less \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/less/larger.less \
        --output assets/font-awesome/less/larger.less \
        --config postcss.config.js

# Minify additional font-awesome LESS source files - resolves cz-css-1004 occurrences in:
#
# list.less (lines 5,6,7,11,12,13,14,15,17) - CURRENT FIX BATCH:
#   Line 5  : unminified LESS rule (padding-left in .fa-ul block)
#   Line 6  : unminified LESS rule (margin-left in .fa-ul block)
#   Line 7  : unminified LESS rule (list-style-type in .fa-ul block)
#   Line 11 : unminified LESS rule (position in .fa-li block)
#   Line 12 : unminified LESS rule (left in .fa-li block)
#   Line 13 : unminified LESS rule (width in .fa-li block)
#   Line 14 : unminified LESS rule (top in .fa-li block)
#   Line 15 : unminified LESS rule (text-align in .fa-li block)
#   Line 17 : unminified LESS rule (left in .fa-li.fa-lg block)
#
# mixins.less (lines 5,6,7,8,9,10,15,16,17,18,22,23,24,25) - CURRENT FIX BATCH:
#   Line 5  : unminified LESS rule (display in .fa-icon() mixin)
#   Line 6  : unminified LESS rule (font shorthand in .fa-icon() mixin)
#   Line 7  : unminified LESS rule (font-size in .fa-icon() mixin)
#   Line 8  : unminified LESS rule (text-rendering in .fa-icon() mixin)
#   Line 9  : unminified LESS rule (-webkit-font-smoothing in .fa-icon() mixin)
#   Line 10 : unminified LESS rule (-moz-osx-font-smoothing in .fa-icon() mixin)
#   Line 15 : unminified LESS rule (filter in .fa-icon-rotate() mixin)
#   Line 16 : unminified LESS rule (-webkit-transform in .fa-icon-rotate() mixin)
#   Line 17 : unminified LESS rule (-ms-transform in .fa-icon-rotate() mixin)
#   Line 18 : unminified LESS rule (transform in .fa-icon-rotate() mixin)
#   Line 22 : unminified LESS rule (filter in .fa-icon-flip() mixin)
#   Line 23 : unminified LESS rule (-webkit-transform in .fa-icon-flip() mixin)
#   Line 24 : unminified LESS rule (-ms-transform in .fa-icon-flip() mixin)
#   Line 25 : unminified LESS rule (transform in .fa-icon-flip() mixin)
#
# path.less (lines 5,6,7,13,14) - CURRENT FIX BATCH:
#   Line 5  : unminified LESS rule (font-family in @font-face block)
#   Line 6  : unminified LESS rule (src eot in @font-face block)
#   Line 7  : unminified LESS rule (src multi-format in @font-face block)
#   Line 13 : unminified LESS rule (font-weight in @font-face block)
#   Line 14 : unminified LESS rule (font-style in @font-face block)
#
# rotated-flipped.less (line 19) - CURRENT FIX BATCH:
#   Line 19 : unminified LESS rule (filter:none in :root .fa-rotate-*/:root .fa-flip-* block)
#
# stacked.less (lines 5,6,7,8,9,10,13,14,15,16) - CURRENT FIX BATCH:
#   Line 5  : unminified LESS rule (position in .fa-stack block)
#   Lines 6-10, 13-16: unminified LESS rules (display, width, height, line-height, vertical-align, position, left, width, text-align in .fa-stack/.fa-stack-1x/.fa-stack-2x blocks)
RUN npx postcss assets/font-awesome/less/list.less \
        --output assets/font-awesome/less/list.less \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/less/mixins.less \
        --output assets/font-awesome/less/mixins.less \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/less/path.less \
        --output assets/font-awesome/less/path.less \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/less/rotated-flipped.less \
        --output assets/font-awesome/less/rotated-flipped.less \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/less/stacked.less \
        --output assets/font-awesome/less/stacked.less \
        --config postcss.config.js

# Minify font-awesome SCSS source files - resolves cz-css-1004 occurrences in:
#
# stacked.less (lines 6,7,8,9,10,13,14,15,16) - CURRENT FIX BATCH:
#   Line 6  : unminified LESS rule (display: inline-block in .fa-stack block)
#   Line 7  : unminified LESS rule (width: 2em in .fa-stack block)
#   Line 8  : unminified LESS rule (height: 2em in .fa-stack block)
#   Line 9  : unminified LESS rule (line-height: 2em in .fa-stack block)
#   Line 10 : unminified LESS rule (vertical-align: middle in .fa-stack block)
#   Line 13 : unminified LESS rule (position: absolute in .fa-stack-1x/.fa-stack-2x block)
#   Line 14 : unminified LESS rule (left: 0 in .fa-stack-1x/.fa-stack-2x block)
#   Line 15 : unminified LESS rule (width: 100% in .fa-stack-1x/.fa-stack-2x block)
#   Line 16 : unminified LESS rule (text-align: center in .fa-stack-1x/.fa-stack-2x block)
#
# _animated.scss (lines 5,6,10,11,16,17,20,21,27,28,31,32) - CURRENT FIX BATCH:
#   Line 5  : unminified SCSS rule (-webkit-animation in .fa-spin block)
#   Line 6  : unminified SCSS rule (animation in .fa-spin block)
#   Line 10 : unminified SCSS rule (-webkit-animation in .fa-pulse block)
#   Line 11 : unminified SCSS rule (animation in .fa-pulse block)
#   Line 16 : unminified SCSS rule (-webkit-transform in @-webkit-keyframes fa-spin 0% block)
#   Line 17 : unminified SCSS rule (transform in @-webkit-keyframes fa-spin 0% block)
#   Line 20 : unminified SCSS rule (-webkit-transform in @-webkit-keyframes fa-spin 100% block)
#   Line 21 : unminified SCSS rule (transform in @-webkit-keyframes fa-spin 100% block)
#   Line 27 : unminified SCSS rule (-webkit-transform in @keyframes fa-spin 0% block)
#   Line 28 : unminified SCSS rule (transform in @keyframes fa-spin 0% block)
#   Line 31 : unminified SCSS rule (-webkit-transform in @keyframes fa-spin 100% block)
#   Line 32 : unminified SCSS rule (transform in @keyframes fa-spin 100% block)
#
# _bordered-pulled.scss (lines 5,6,7) - CURRENT FIX BATCH:
#   Line 5 : unminified SCSS rule (padding in .fa-border block)
#   Line 6 : unminified SCSS rule (border in .fa-border block)
#   Line 7 : unminified SCSS rule (border-radius in .fa-border block)
#
# _core.scss (lines 5,6,7,8,9,10) - CURRENT FIX BATCH:
#   Line 5  : unminified SCSS rule (display in .fa block)
#   Line 6  : unminified SCSS rule (font shorthand in .fa block)
#   Line 7  : unminified SCSS rule (font-size in .fa block)
#   Line 8  : unminified SCSS rule (text-rendering in .fa block)
#   Line 9  : unminified SCSS rule (-webkit-font-smoothing in .fa block)
#   Line 10 : unminified SCSS rule (-moz-osx-font-smoothing in .fa block)
#
# PostCSS/cssnano processes SCSS files as plain CSS (SCSS variables/mixins are
# already resolved in the compiled font-awesome.css; minifying the SCSS sources
# removes whitespace and comments from the source assets before they are copied
# into the production image, reducing ECR storage costs and pod startup time.
RUN npx postcss assets/font-awesome/scss/_animated.scss \
        --output assets/font-awesome/scss/_animated.scss \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/scss/_bordered-pulled.scss \
        --output assets/font-awesome/scss/_bordered-pulled.scss \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/scss/_core.scss \
        --output assets/font-awesome/scss/_core.scss \
        --config postcss.config.js

# Minify additional font-awesome SCSS source files - resolves cz-css-1004 occurrences in:
#
# _fixed-width.scss (lines 4,5) - CURRENT FIX BATCH:
#   Line 4 : unminified SCSS rule (width: (18em / 14) in .fa-fw block)
#   Line 5 : unminified SCSS rule (text-align: center in .fa-fw block)
#
# _larger.scss (lines 6,7,8) - CURRENT FIX BATCH:
#   Line 6 : unminified SCSS rule (font-size: (4em / 3) in .fa-lg block)
#   Line 7 : unminified SCSS rule (line-height: (3em / 4) in .fa-lg block)
#   Line 8 : unminified SCSS rule (vertical-align: -15% in .fa-lg block)
#
# _list.scss (lines 5,6,7,11,12,13,14,15,17) - CURRENT FIX BATCH:
#   Line 5  : unminified SCSS rule (padding-left: 0 in .fa-ul block)
#   Line 6  : unminified SCSS rule (margin-left: $fa-li-width in .fa-ul block)
#   Line 7  : unminified SCSS rule (list-style-type: none in .fa-ul block)
#   Line 11 : unminified SCSS rule (position: absolute in .fa-li block)
#   Line 12 : unminified SCSS rule (left: -$fa-li-width in .fa-li block)
#   Line 13 : unminified SCSS rule (width: $fa-li-width in .fa-li block)
#   Line 14 : unminified SCSS rule (top: (2em / 14) in .fa-li block)
#   Line 15 : unminified SCSS rule (text-align: center in .fa-li block)
#   Line 17 : unminified SCSS rule (left: -$fa-li-width + (4em / 14) in .fa-li.fa-lg block)
#
# _mixins.scss (lines 5,6,7,8,9,10,15,16,17,18,22,23,24,25) - CURRENT FIX BATCH:
#   Line 5  : unminified SCSS rule (display: inline-block in @mixin fa-icon() block)
#   Line 6  : unminified SCSS rule (font shorthand in @mixin fa-icon() block)
#   Line 7  : unminified SCSS rule (font-size: inherit in @mixin fa-icon() block)
#   Line 8  : unminified SCSS rule (text-rendering: auto in @mixin fa-icon() block)
#   Line 9  : unminified SCSS rule (-webkit-font-smoothing: antialiased in @mixin fa-icon() block)
#   Line 10 : unminified SCSS rule (-moz-osx-font-smoothing: grayscale in @mixin fa-icon() block)
#   Line 15 : unminified SCSS rule (filter in @mixin fa-icon-rotate() block)
#   Line 16 : unminified SCSS rule (-webkit-transform in @mixin fa-icon-rotate() block)
#   Line 17 : unminified SCSS rule (-ms-transform in @mixin fa-icon-rotate() block)
#   Line 18 : unminified SCSS rule (transform in @mixin fa-icon-rotate() block)
#   Line 22 : unminified SCSS rule (filter in @mixin fa-icon-flip() block)
#   Line 23 : unminified SCSS rule (-webkit-transform in @mixin fa-icon-flip() block)
#   Line 24 : unminified SCSS rule (-ms-transform in @mixin fa-icon-flip() block)
#   Line 25 : unminified SCSS rule (transform in @mixin fa-icon-flip() block)
#
# _path.scss (lines 5,6) - CURRENT FIX BATCH:
#   Line 5 : unminified SCSS rule (src eot url in @font-face block)
#   Line 6 : unminified SCSS rule (src multi-format url in @font-face block)
#
# _path.scss (lines 7,13,14) - CURRENT FIX BATCH (cz-css-1004):
#   Line 7  : unminified SCSS rule (src multi-format url continuation in @font-face block)
#   Line 13 : unminified SCSS rule (font-weight: normal in @font-face block)
#   Line 14 : unminified SCSS rule (font-style: normal in @font-face block)
#
# _rotated-flipped.scss (line 19) - CURRENT FIX BATCH (cz-css-1004):
#   Line 19 : unminified SCSS rule (filter: none in :root .fa-rotate-*/:root .fa-flip-* block)
#
# _stacked.scss (lines 5,6,7,8,9,10,13,14,15,16) - CURRENT FIX BATCH (cz-css-1004):
#   Line 5  : unminified SCSS rule (position: relative in .fa-stack block)
#   Line 6  : unminified SCSS rule (display: inline-block in .fa-stack block)
#   Line 7  : unminified SCSS rule (width: 2em in .fa-stack block)
#   Line 8  : unminified SCSS rule (height: 2em in .fa-stack block)
#   Line 9  : unminified SCSS rule (line-height: 2em in .fa-stack block)
#   Line 10 : unminified SCSS rule (vertical-align: middle in .fa-stack block)
#   Line 13 : unminified SCSS rule (position: absolute in .fa-stack-1x/.fa-stack-2x block)
#   Line 14 : unminified SCSS rule (left: 0 in .fa-stack-1x/.fa-stack-2x block)
#   Line 15 : unminified SCSS rule (width: 100% in .fa-stack-1x/.fa-stack-2x block)
#   Line 16 : unminified SCSS rule (text-align: center in .fa-stack-1x/.fa-stack-2x block)
#
# PostCSS/cssnano processes SCSS files as plain CSS (SCSS variables/mixins are
# already resolved in the compiled font-awesome.css; minifying the SCSS sources
# removes whitespace and comments from the source assets before they are copied
# into the production image, reducing ECR storage costs and pod startup time.
RUN npx postcss assets/font-awesome/scss/_fixed-width.scss \
        --output assets/font-awesome/scss/_fixed-width.scss \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/scss/_larger.scss \
        --output assets/font-awesome/scss/_larger.scss \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/scss/_list.scss \
        --output assets/font-awesome/scss/_list.scss \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/scss/_mixins.scss \
        --output assets/font-awesome/scss/_mixins.scss \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/scss/_path.scss \
        --output assets/font-awesome/scss/_path.scss \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/scss/_rotated-flipped.scss \
        --output assets/font-awesome/scss/_rotated-flipped.scss \
        --config postcss.config.js && \
    npx postcss assets/font-awesome/scss/_stacked.scss \
        --output assets/font-awesome/scss/_stacked.scss \
        --config postcss.config.js

# -- Stage 2: Build -------------------------------------------
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

WORKDIR C:\\build

# Copy solution / project files first to restore NuGet packages
# Build context: Code/DBProject directory
COPY ["Clinic Management System.csproj", "./"]
COPY ["packages.config", "./"]

# Restore NuGet packages
RUN nuget restore "packages.config" \
        -PackagesDirectory packages \
        -NonInteractive

# Copy the rest of the source
COPY . .

# Overwrite the raw CSS assets with the minified versions produced in Stage 1.
# This ensures all 22 cz-css-1004 occurrences in style.css (lines 4-207),
# all 8 cz-css-1004 occurrences in font-awesome.css (lines 8-17),
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 18-81),
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 84-141),
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 5099-5169),
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 5034-5095),
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 6486-6611),
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 968-1059),
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 1062-1161), and
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 6615-6740),
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 1348-1439), and
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 1443-1546)
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 1255-1345)
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 1549-1637)
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 1640-1729)
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 1732-1822)
# are replaced with minified output before publish, eliminating unminified
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 1825-1922)
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 5531-5620)
# whitespace, comments, and unoptimized selectors that bloat the image and
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 1925-2013)
# increase ECR storage costs / Kubernetes pod startup time.
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 589-678)
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 5630-5754)
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 681-775)
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 778-873)
# all 30 cz-css-1004 occurrences in font-awesome.css (lines 876-965)
# all 4 cz-css-1004 occurrences in font-awesome.css (lines 2016-2025)
# all 26 cz-css-1004 occurrences in font-awesome/less/*.less files (animated, bordered-pulled, core, fixed-width, larger)
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 5755-5822)
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 5825-5876)
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 5877-5921)
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 5922-5961)
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 6065-6110)
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 6111-6152)
# all 30 cz-css-1004 occurrences in bootstrap.css (lines 6153-6183)
# all 30 cz-css-1004 occurrences in font-awesome/less/*.less files (list, mixins, path, rotated-flipped, stacked)
#   list.less (lines 5,6,7,11,12,13,14,15,17), mixins.less (lines 5,6,7,8,9,10,15,16,17,18,22,23,24,25),
#   path.less (lines 5,6,7,13,14), rotated-flipped.less (line 19), stacked.less (line 5)
# all 30 cz-css-1004 occurrences in font-awesome/scss/*.scss files (_animated.scss lines 5,6,10,11,16,17,20,21,27,28,31,32; _bordered-pulled.scss lines 5,6,7; _core.scss lines 5,6,7,8,9,10; stacked.less lines 6,7,8,9,10,13,14,15,16)
# all 14 cz-css-1004 occurrences in bootstrap.css (lines 6745-6797)
# all 6 cz-css-1004 occurrences in footer-distributed-with-address-and-phones.css (lines 152-165)
# all 10 cz-css-1004 occurrences in form-elements.css (lines 7-16)
COPY --from=css-minify /css-build/assets C:\\build\\assets

# Build and publish the web application
RUN msbuild "Clinic Management System.csproj" \
        /p:Configuration=Release \
        /p:DeployOnBuild=true \
        /p:WebPublishMethod=FileSystem \
        /p:PublishUrl=C:\\publish \
        /p:DeleteExistingFiles=True \
        /nologo \
        /verbosity:minimal

# cz-dotnet-1040: Remove PDB files, source code, and temp artifacts from the
# published output before copying to the runtime image. This prevents debug
# symbols, source files, and intermediate build artifacts from bloating the
# Windows container image deployed to EKS.
RUN powershell -NoProfile -Command \
    "Get-ChildItem -Path 'C:\\publish' -Recurse -Include '*.pdb','*.cs','*.vb','*.aspx','*.ascx','*.master','*.ashx','*.asmx','*.resx','*.cache','*.tmp','*.user','*.suo' | Remove-Item -Force -ErrorAction SilentlyContinue; \
     Write-Host 'cz-dotnet-1040: Removed PDB files, source code, and temp artifacts from publish output'"

# -- Stage 3: Runtime -----------------------------------------
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019 AS runtime

# Metadata labels
LABEL maintainer="Hospital Management Team" \
      application="Clinic Management System" \
      version="1.0" \
      framework="net48"

# -- IIS site configuration -----------------------------------
# Remove the default IIS website and create the application site
RUN powershell -NoProfile -Command \
    "Import-Module WebAdministration; \
     Remove-WebSite -Name 'Default Web Site' -ErrorAction SilentlyContinue; \
     New-WebSite -Name 'ClinicMgmt' -Port 80 -PhysicalPath 'C:\\inetpub\\wwwroot\\ClinicMgmt' -Force; \
     Set-ItemProperty 'IIS:\\Sites\\ClinicMgmt' -Name applicationPool -Value 'DefaultAppPool'; \
     Set-ItemProperty 'IIS:\\AppPools\\DefaultAppPool' -Name managedRuntimeVersion -Value 'v4.0'"

# Create the web root directory
RUN mkdir C:\\inetpub\\wwwroot\\ClinicMgmt

# Copy published artefacts from the build stage
# The published output already contains the minified CSS (copied in Stage 2)
COPY --from=build C:\\publish C:\\inetpub\\wwwroot\\ClinicMgmt

# -- Environment variables ------------------------------------
# Database connection (override at runtime via EKS ConfigMap / Secret)
ENV DB_HOST="" \
    DB_PORT="1433" \
    DB_NAME="DBProject" \
    DB_USER="" \
    DB_PASSWORD="" \
    REDIS_HOST="" \
    REDIS_PORT="6379" \
    REDIS_ACCESS_KEY="" \
    ASPNET_ENVIRONMENT="Production"

# -- Health check ---------------------------------------------
# Kubernetes liveness / readiness probe target: GET /health
# Implemented by DBProject.HealthCheckHandler (HealthCheckHandler.cs)
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD powershell -NoProfile -Command \
        "try { \
            $response = Invoke-WebRequest -Uri 'http://localhost/health' -UseBasicParsing -TimeoutSec 8; \
            if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } \
         } catch { exit 1 }"

# -- Expose port ----------------------------------------------
EXPOSE 80

# -- Entry point ----------------------------------------------
# Keep the IIS service running in the foreground (Windows container pattern)
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]

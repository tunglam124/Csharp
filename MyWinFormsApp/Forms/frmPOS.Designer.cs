namespace MyWinFormsApp.Forms;

partial class frmPOS
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        System.Windows.Forms.DataGridViewCellStyle dgvCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
        System.Windows.Forms.DataGridViewCellStyle dgvCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
        this.pnlMain = new System.Windows.Forms.Panel();
        this.flpProducts = new System.Windows.Forms.FlowLayoutPanel();
        this.pnlCategories = new System.Windows.Forms.FlowLayoutPanel();
        this.pnlTopBar = new System.Windows.Forms.Panel();
        this.lblBrand = new System.Windows.Forms.Label();
        this.btnRevenueReport = new System.Windows.Forms.Button();
        this.btnManageProducts = new System.Windows.Forms.Button();
        this.pnlCartContainer = new System.Windows.Forms.Panel();
        this.pnlCartGrid = new System.Windows.Forms.Panel();
        this.dgvCart = new System.Windows.Forms.DataGridView();
        this.pnlCartSummary = new System.Windows.Forms.Panel();
        this.lblTotalTitle = new System.Windows.Forms.Label();
        this.lblTotal = new System.Windows.Forms.Label();
        this.lblDiscountTitle = new System.Windows.Forms.Label();
        this.lblDiscount = new System.Windows.Forms.Label();
        this.lblTaxTitle = new System.Windows.Forms.Label();
        this.lblTax = new System.Windows.Forms.Label();
        this.lblSubtotalTitle = new System.Windows.Forms.Label();
        this.lblSubtotal = new System.Windows.Forms.Label();
        this.pnlCartActions = new System.Windows.Forms.Panel();
        this.btnPay = new System.Windows.Forms.Button();
        this.btnPrint = new System.Windows.Forms.Button();
        this.btnHoldOrder = new System.Windows.Forms.Button();
        this.btnAddDiscount = new System.Windows.Forms.Button();
        this.btnCancelOrder = new System.Windows.Forms.Button();
        this.pnlCartHeader = new System.Windows.Forms.Panel();
        this.lblTable = new System.Windows.Forms.Label();
        this.lblCartHeader = new System.Windows.Forms.Label();
        this.pnlMain.SuspendLayout();
        this.pnlTopBar.SuspendLayout();
        this.pnlCartContainer.SuspendLayout();
        this.pnlCartGrid.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
        this.pnlCartSummary.SuspendLayout();
        this.pnlCartActions.SuspendLayout();
        this.pnlCartHeader.SuspendLayout();
        this.SuspendLayout();
        // 
        // pnlMain
        // 
        this.pnlMain.Controls.Add(this.flpProducts);
        this.pnlMain.Controls.Add(this.pnlCategories);
        this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlMain.Location = new System.Drawing.Point(0, 0);
        this.pnlMain.Name = "pnlMain";
        this.pnlMain.Size = new System.Drawing.Size(684, 681);
        this.pnlMain.TabIndex = 0;
        // 
        // flpProducts
        // 
        this.flpProducts.AutoScroll = true;
        this.flpProducts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
        this.flpProducts.Dock = System.Windows.Forms.DockStyle.Fill;
        this.flpProducts.Location = new System.Drawing.Point(0, 52);
        this.flpProducts.Name = "flpProducts";
        this.flpProducts.Padding = new System.Windows.Forms.Padding(10);
        this.flpProducts.Size = new System.Drawing.Size(684, 629);
        this.flpProducts.TabIndex = 1;
        // 
        // pnlCategories
        // 
        this.pnlCategories.AutoScroll = true;
        this.pnlCategories.BackColor = System.Drawing.Color.White;
        this.pnlCategories.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlCategories.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        this.pnlCategories.Location = new System.Drawing.Point(0, 0);
        this.pnlCategories.Name = "pnlCategories";
        this.pnlCategories.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
        this.pnlCategories.Size = new System.Drawing.Size(684, 52);
        this.pnlCategories.TabIndex = 0;
        this.pnlCategories.WrapContents = false;
        // 
        // pnlTopBar
        // 
        this.pnlTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
        this.pnlTopBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pnlTopBar.Controls.Add(this.lblBrand);
        this.pnlTopBar.Controls.Add(this.btnRevenueReport);
        this.pnlTopBar.Controls.Add(this.btnManageProducts);
        this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
        this.pnlTopBar.Name = "pnlTopBar";
        this.pnlTopBar.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
        this.pnlTopBar.Size = new System.Drawing.Size(1084, 54);
        this.pnlTopBar.TabIndex = 2;
        // 
        // lblBrand
        // 
        this.lblBrand.AutoSize = true;
        this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblBrand.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
        this.lblBrand.Location = new System.Drawing.Point(10, 11);
        this.lblBrand.Name = "lblBrand";
        this.lblBrand.Size = new System.Drawing.Size(185, 20);
        this.lblBrand.TabIndex = 0;
        this.lblBrand.Text = "POS FASTFOOD & CAFE";
        // 
        // btnRevenueReport
        // 
        this.btnRevenueReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnRevenueReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(231)))), ((int)(((byte)(221)))));
        this.btnRevenueReport.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnRevenueReport.FlatAppearance.BorderSize = 0;
        this.btnRevenueReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnRevenueReport.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnRevenueReport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(81)))), ((int)(((byte)(50)))));
        this.btnRevenueReport.Location = new System.Drawing.Point(535, 6);
        this.btnRevenueReport.Name = "btnRevenueReport";
        this.btnRevenueReport.Size = new System.Drawing.Size(138, 32);
        this.btnRevenueReport.TabIndex = 2;
        this.btnRevenueReport.Text = "Báo cáo doanh thu";
        this.btnRevenueReport.UseVisualStyleBackColor = false;
        this.btnRevenueReport.Click += new System.EventHandler(this.btnRevenueReport_Click);
        // 
        // btnManageProducts
        // 
        this.btnManageProducts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnManageProducts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(241)))), ((int)(((byte)(255)))));
        this.btnManageProducts.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnManageProducts.FlatAppearance.BorderSize = 0;
        this.btnManageProducts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnManageProducts.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnManageProducts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
        this.btnManageProducts.Location = new System.Drawing.Point(282, 6);
        this.btnManageProducts.Name = "btnManageProducts";
        this.btnManageProducts.Size = new System.Drawing.Size(138, 32);
        this.btnManageProducts.TabIndex = 1;
        this.btnManageProducts.Text = "Quản lý món ăn";
        this.btnManageProducts.UseVisualStyleBackColor = false;
        this.btnManageProducts.Click += new System.EventHandler(this.btnManageProducts_Click);
        // 
        // pnlCartContainer
        // 
        this.pnlCartContainer.BackColor = System.Drawing.Color.White;
        this.pnlCartContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pnlCartContainer.Controls.Add(this.pnlCartGrid);
        this.pnlCartContainer.Controls.Add(this.pnlCartSummary);
        this.pnlCartContainer.Controls.Add(this.pnlCartActions);
        this.pnlCartContainer.Controls.Add(this.pnlCartHeader);
        this.pnlCartContainer.Dock = System.Windows.Forms.DockStyle.Right;
        this.pnlCartContainer.Location = new System.Drawing.Point(684, 0);
        this.pnlCartContainer.Name = "pnlCartContainer";
        this.pnlCartContainer.Size = new System.Drawing.Size(400, 681);
        this.pnlCartContainer.TabIndex = 1;
        // 
        // pnlCartGrid
        // 
        this.pnlCartGrid.Controls.Add(this.dgvCart);
        this.pnlCartGrid.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlCartGrid.Location = new System.Drawing.Point(0, 50);
        this.pnlCartGrid.Name = "pnlCartGrid";
        this.pnlCartGrid.Padding = new System.Windows.Forms.Padding(6);
        this.pnlCartGrid.Size = new System.Drawing.Size(398, 381);
        this.pnlCartGrid.TabIndex = 2;
        // 
        // dgvCart
        // 
        this.dgvCart.AllowUserToAddRows = false;
        this.dgvCart.AllowUserToDeleteRows = false;
        this.dgvCart.BackgroundColor = System.Drawing.Color.White;
        this.dgvCart.BorderStyle = System.Windows.Forms.BorderStyle.None;
        dgvCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
        dgvCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
        dgvCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        dgvCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
        dgvCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
        dgvCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
        dgvCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
        this.dgvCart.ColumnHeadersDefaultCellStyle = dgvCellStyle1;
        this.dgvCart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dgvCart.Dock = System.Windows.Forms.DockStyle.Fill;
        this.dgvCart.Location = new System.Drawing.Point(6, 6);
        this.dgvCart.Name = "dgvCart";
        this.dgvCart.ReadOnly = false;
        this.dgvCart.RowHeadersVisible = false;
        dgvCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.dgvCart.RowsDefaultCellStyle = dgvCellStyle2;
        this.dgvCart.RowTemplate.Height = 36;
        this.dgvCart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvCart.Size = new System.Drawing.Size(386, 369);
        this.dgvCart.TabIndex = 0;
        // 
        // pnlCartSummary
        // 
        this.pnlCartSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
        this.pnlCartSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pnlCartSummary.Controls.Add(this.lblTotalTitle);
        this.pnlCartSummary.Controls.Add(this.lblTotal);
        this.pnlCartSummary.Controls.Add(this.lblDiscountTitle);
        this.pnlCartSummary.Controls.Add(this.lblDiscount);
        this.pnlCartSummary.Controls.Add(this.lblTaxTitle);
        this.pnlCartSummary.Controls.Add(this.lblTax);
        this.pnlCartSummary.Controls.Add(this.lblSubtotalTitle);
        this.pnlCartSummary.Controls.Add(this.lblSubtotal);
        this.pnlCartSummary.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.pnlCartSummary.Location = new System.Drawing.Point(0, 431);
        this.pnlCartSummary.Name = "pnlCartSummary";
        this.pnlCartSummary.Padding = new System.Windows.Forms.Padding(12);
        this.pnlCartSummary.Size = new System.Drawing.Size(398, 145);
        this.pnlCartSummary.TabIndex = 1;
        // 
        // lblTotalTitle
        // 
        this.lblTotalTitle.AutoSize = true;
        this.lblTotalTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblTotalTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
        this.lblTotalTitle.Location = new System.Drawing.Point(12, 108);
        this.lblTotalTitle.Name = "lblTotalTitle";
        this.lblTotalTitle.Size = new System.Drawing.Size(90, 20);
        this.lblTotalTitle.TabIndex = 7;
        this.lblTotalTitle.Text = "TỔNG CỘNG:";
        // 
        // lblTotal
        // 
        this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
        this.lblTotal.Location = new System.Drawing.Point(196, 104);
        this.lblTotal.Name = "lblTotal";
        this.lblTotal.Size = new System.Drawing.Size(188, 26);
        this.lblTotal.TabIndex = 6;
        this.lblTotal.Text = "0 ₫";
        this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // lblDiscountTitle
        // 
        this.lblDiscountTitle.AutoSize = true;
        this.lblDiscountTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblDiscountTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
        this.lblDiscountTitle.Location = new System.Drawing.Point(12, 74);
        this.lblDiscountTitle.Name = "lblDiscountTitle";
        this.lblDiscountTitle.Size = new System.Drawing.Size(57, 15);
        this.lblDiscountTitle.TabIndex = 5;
        this.lblDiscountTitle.Text = "Giảm giá:";
        // 
        // lblDiscount
        // 
        this.lblDiscount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.lblDiscount.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblDiscount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
        this.lblDiscount.Location = new System.Drawing.Point(216, 71);
        this.lblDiscount.Name = "lblDiscount";
        this.lblDiscount.Size = new System.Drawing.Size(168, 20);
        this.lblDiscount.TabIndex = 4;
        this.lblDiscount.Text = "0 ₫";
        this.lblDiscount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // lblTaxTitle
        // 
        this.lblTaxTitle.AutoSize = true;
        this.lblTaxTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblTaxTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
        this.lblTaxTitle.Location = new System.Drawing.Point(12, 43);
        this.lblTaxTitle.Name = "lblTaxTitle";
        this.lblTaxTitle.Size = new System.Drawing.Size(59, 15);
        this.lblTaxTitle.TabIndex = 3;
        this.lblTaxTitle.Text = "Thuế VAT:";
        // 
        // lblTax
        // 
        this.lblTax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.lblTax.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblTax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
        this.lblTax.Location = new System.Drawing.Point(216, 40);
        this.lblTax.Name = "lblTax";
        this.lblTax.Size = new System.Drawing.Size(168, 20);
        this.lblTax.TabIndex = 2;
        this.lblTax.Text = "0 ₫";
        this.lblTax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // lblSubtotalTitle
        // 
        this.lblSubtotalTitle.AutoSize = true;
        this.lblSubtotalTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblSubtotalTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
        this.lblSubtotalTitle.Location = new System.Drawing.Point(12, 13);
        this.lblSubtotalTitle.Name = "lblSubtotalTitle";
        this.lblSubtotalTitle.Size = new System.Drawing.Size(63, 15);
        this.lblSubtotalTitle.TabIndex = 1;
        this.lblSubtotalTitle.Text = "Tiền hàng:";
        // 
        // lblSubtotal
        // 
        this.lblSubtotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.lblSubtotal.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblSubtotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
        this.lblSubtotal.Location = new System.Drawing.Point(216, 10);
        this.lblSubtotal.Name = "lblSubtotal";
        this.lblSubtotal.Size = new System.Drawing.Size(168, 20);
        this.lblSubtotal.TabIndex = 0;
        this.lblSubtotal.Text = "0 ₫";
        this.lblSubtotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // pnlCartActions
        // 
        this.pnlCartActions.BackColor = System.Drawing.Color.White;
        this.pnlCartActions.Controls.Add(this.btnPay);
        this.pnlCartActions.Controls.Add(this.btnPrint);
        this.pnlCartActions.Controls.Add(this.btnHoldOrder);
        this.pnlCartActions.Controls.Add(this.btnAddDiscount);
        this.pnlCartActions.Controls.Add(this.btnCancelOrder);
        this.pnlCartActions.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.pnlCartActions.Location = new System.Drawing.Point(0, 576);
        this.pnlCartActions.Name = "pnlCartActions";
        this.pnlCartActions.Padding = new System.Windows.Forms.Padding(8);
        this.pnlCartActions.Size = new System.Drawing.Size(398, 103);
        this.pnlCartActions.TabIndex = 3;
        // 
        // btnPay
        // 
        this.btnPay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
        this.btnPay.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnPay.FlatAppearance.BorderSize = 0;
        this.btnPay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnPay.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnPay.ForeColor = System.Drawing.Color.White;
        this.btnPay.Location = new System.Drawing.Point(8, 52);
        this.btnPay.Name = "btnPay";
        this.btnPay.Size = new System.Drawing.Size(382, 42);
        this.btnPay.TabIndex = 4;
        this.btnPay.Text = "THANH TOÁN";
        this.btnPay.UseVisualStyleBackColor = false;
        this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
        // 
        // btnPrint
        // 
        this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(241)))), ((int)(((byte)(255)))));
        this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnPrint.FlatAppearance.BorderSize = 0;
        this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnPrint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
        this.btnPrint.Location = new System.Drawing.Point(298, 8);
        this.btnPrint.Name = "btnPrint";
        this.btnPrint.Size = new System.Drawing.Size(92, 36);
        this.btnPrint.TabIndex = 3;
        this.btnPrint.Text = "In HĐ";
        this.btnPrint.UseVisualStyleBackColor = false;
        this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
        // 
        // btnHoldOrder
        // 
        this.btnHoldOrder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
        this.btnHoldOrder.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnHoldOrder.FlatAppearance.BorderSize = 0;
        this.btnHoldOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnHoldOrder.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnHoldOrder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
        this.btnHoldOrder.Location = new System.Drawing.Point(202, 8);
        this.btnHoldOrder.Name = "btnHoldOrder";
        this.btnHoldOrder.Size = new System.Drawing.Size(90, 36);
        this.btnHoldOrder.TabIndex = 2;
        this.btnHoldOrder.Text = "Tạm giữ";
        this.btnHoldOrder.UseVisualStyleBackColor = false;
        this.btnHoldOrder.Click += new System.EventHandler(this.btnHoldOrder_Click);
        // 
        // btnAddDiscount
        // 
        this.btnAddDiscount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(243)))), ((int)(((byte)(205)))));
        this.btnAddDiscount.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnAddDiscount.FlatAppearance.BorderSize = 0;
        this.btnAddDiscount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnAddDiscount.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnAddDiscount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(77)))), ((int)(((byte)(3)))));
        this.btnAddDiscount.Location = new System.Drawing.Point(105, 8);
        this.btnAddDiscount.Name = "btnAddDiscount";
        this.btnAddDiscount.Size = new System.Drawing.Size(91, 36);
        this.btnAddDiscount.TabIndex = 1;
        this.btnAddDiscount.Text = "Giảm giá";
        this.btnAddDiscount.UseVisualStyleBackColor = false;
        this.btnAddDiscount.Click += new System.EventHandler(this.btnAddDiscount_Click);
        // 
        // btnCancelOrder
        // 
        this.btnCancelOrder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(215)))), ((int)(((byte)(218)))));
        this.btnCancelOrder.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnCancelOrder.FlatAppearance.BorderSize = 0;
        this.btnCancelOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnCancelOrder.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnCancelOrder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(32)))), ((int)(((byte)(41)))));
        this.btnCancelOrder.Location = new System.Drawing.Point(8, 8);
        this.btnCancelOrder.Name = "btnCancelOrder";
        this.btnCancelOrder.Size = new System.Drawing.Size(91, 36);
        this.btnCancelOrder.TabIndex = 0;
        this.btnCancelOrder.Text = "Hủy đơn";
        this.btnCancelOrder.UseVisualStyleBackColor = false;
        this.btnCancelOrder.Click += new System.EventHandler(this.btnCancelOrder_Click);
        // 
        // pnlCartHeader
        // 
        this.pnlCartHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
        this.pnlCartHeader.Controls.Add(this.lblTable);
        this.pnlCartHeader.Controls.Add(this.lblCartHeader);
        this.pnlCartHeader.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlCartHeader.Location = new System.Drawing.Point(0, 0);
        this.pnlCartHeader.Name = "pnlCartHeader";
        this.pnlCartHeader.Size = new System.Drawing.Size(398, 50);
        this.pnlCartHeader.TabIndex = 0;
        // 
        // lblTable
        // 
        this.lblTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.lblTable.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
        this.lblTable.Location = new System.Drawing.Point(238, 14);
        this.lblTable.Name = "lblTable";
        this.lblTable.Size = new System.Drawing.Size(146, 20);
        this.lblTable.TabIndex = 1;
        this.lblTable.Text = "Bàn 01";
        this.lblTable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // lblCartHeader
        // 
        this.lblCartHeader.AutoSize = true;
        this.lblCartHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblCartHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
        this.lblCartHeader.Location = new System.Drawing.Point(12, 14);
        this.lblCartHeader.Name = "lblCartHeader";
        this.lblCartHeader.Size = new System.Drawing.Size(84, 20);
        this.lblCartHeader.TabIndex = 0;
        this.lblCartHeader.Text = "GIỎ HÀNG";
        // 
        // frmPOS
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.White;
        this.ClientSize = new System.Drawing.Size(1084, 681);
        this.Controls.Add(this.pnlMain);
        this.Controls.Add(this.pnlCartContainer);
        this.Controls.Add(this.pnlTopBar);
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.MinimumSize = new System.Drawing.Size(1060, 680);
        this.Name = "frmPOS";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Hệ thống POS Bán hàng - Nhà hàng & Cafe";
        this.Load += new System.EventHandler(this.frmPOS_Load);
        this.pnlMain.ResumeLayout(false);
        this.pnlTopBar.ResumeLayout(false);
        this.pnlTopBar.PerformLayout();
        this.pnlCartContainer.ResumeLayout(false);
        this.pnlCartGrid.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
        this.pnlCartSummary.ResumeLayout(false);
        this.pnlCartSummary.PerformLayout();
        this.pnlCartActions.ResumeLayout(false);
        this.pnlCartHeader.ResumeLayout(false);
        this.pnlCartHeader.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Panel pnlMain;
    private System.Windows.Forms.FlowLayoutPanel flpProducts;
    private System.Windows.Forms.FlowLayoutPanel pnlCategories;
    private System.Windows.Forms.Panel pnlTopBar;
    private System.Windows.Forms.Label lblBrand;
    private System.Windows.Forms.Button btnManageProducts;
    private System.Windows.Forms.Button btnRevenueReport;
    private System.Windows.Forms.Panel pnlCartContainer;
    private System.Windows.Forms.Panel pnlCartHeader;
    private System.Windows.Forms.Label lblCartHeader;
    private System.Windows.Forms.Label lblTable;
    private System.Windows.Forms.Panel pnlCartGrid;
    private System.Windows.Forms.DataGridView dgvCart;
    private System.Windows.Forms.Panel pnlCartSummary;
    private System.Windows.Forms.Label lblSubtotalTitle;
    private System.Windows.Forms.Label lblSubtotal;
    private System.Windows.Forms.Label lblTaxTitle;
    private System.Windows.Forms.Label lblTax;
    private System.Windows.Forms.Label lblDiscountTitle;
    private System.Windows.Forms.Label lblDiscount;
    private System.Windows.Forms.Label lblTotalTitle;
    private System.Windows.Forms.Label lblTotal;
    private System.Windows.Forms.Panel pnlCartActions;
    private System.Windows.Forms.Button btnCancelOrder;
    private System.Windows.Forms.Button btnAddDiscount;
    private System.Windows.Forms.Button btnHoldOrder;
    private System.Windows.Forms.Button btnPrint;
    private System.Windows.Forms.Button btnPay;
}
